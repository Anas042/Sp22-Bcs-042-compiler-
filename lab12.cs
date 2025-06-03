using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LexicalAnalyzerV1
{
    class Token
    {
        public string Type;
        public string Value;
        public int Line;
        public int Column;

        public Token(string type, string value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }
    }

    class Program
    {
        static List<string> keywordList = new List<string> {
            "int", "float", "while", "main", "if", "else", "new"
        };

        static Regex variable_Reg = new Regex(@"^[A-Za-z_][A-Za-z0-9_]*$");
        // Updated number regex to handle exponentials properly
        static Regex constants_Reg = new Regex(@"^[0-9]+(\.[0-9]+)?([eE][+-]?[0-9]+)?$");
        // Extended operator regex for multi-char ops
        static HashSet<string> operatorsSet = new HashSet<string> {
            "+", "-", "*", "/", ">", "<", "=", "==", "!=", ">=", "<=", "&", "|"
        };
        static Regex Special_Reg = new Regex(@"^[.,'\[\]{}();:?]$");

        static List<Token> tokens = new List<Token>();

        static void Main(string[] args)
        {
            Console.WriteLine("Enter code (end with an empty line):");
            string userInput = "";
            string line;

            // Read multi-line input
            while ((line = Console.ReadLine()) != null && line != "")
            {
                userInput += line + "\n";
            }

            // Lexical Analysis
            TokenizeAndPrint(userInput);

            // Parser
            ParseTokens();
        }

        static void TokenizeAndPrint(string input)
        {
            tokens.Clear();
            int line_num = 1;
            int col_num = 1;
            int i = 0;

            while (i < input.Length)
            {
                char c = input[i];

                if (c == '\n')
                {
                    line_num++;
                    col_num = 1;
                    i++;
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    col_num++;
                    i++;
                    continue;
                }

                // Identifier or keyword
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    int startCol = col_num;
                    while (i < input.Length && (char.IsLetterOrDigit(input[i]) || input[i] == '_'))
                    {
                        i++; col_num++;
                    }
                    string word = input.Substring(start, i - start);
                    if (keywordList.Contains(word))
                    {
                        Console.WriteLine($"< keyword, {word} >");
                        tokens.Add(new Token("keyword", word, line_num, startCol));
                    }
                    else
                    {
                        Console.WriteLine($"< id, {word} >");
                        tokens.Add(new Token("id", word, line_num, startCol));
                    }
                    continue;
                }

                // Number (including decimals and exponentials)
                if (char.IsDigit(c))
                {
                    int start = i;
                    int startCol = col_num;
                    while (i < input.Length &&
                        (char.IsDigit(input[i]) || input[i] == '.' ||
                        (input[i] == 'e' || input[i] == 'E') && i + 1 < input.Length &&
                        (char.IsDigit(input[i + 1]) || input[i + 1] == '+' || input[i + 1] == '-')))
                    {
                        i++; col_num++;
                    }
                    string num = input.Substring(start, i - start);
                    Console.WriteLine($"< digit, {num} >");
                    tokens.Add(new Token("digit", num, line_num, startCol));
                    continue;
                }

                // Operators (including multi-char)
                // Try to match two-character operators first
                if (i + 1 < input.Length)
                {
                    string twoCharOp = input.Substring(i, 2);
                    if (operatorsSet.Contains(twoCharOp))
                    {
                        Console.WriteLine($"< op, {twoCharOp} >");
                        tokens.Add(new Token("op", twoCharOp, line_num, col_num));
                        i += 2;
                        col_num += 2;
                        continue;
                    }
                }

                // Single char operator
                string oneCharOp = c.ToString();
                if (operatorsSet.Contains(oneCharOp))
                {
                    Console.WriteLine($"< op, {oneCharOp} >");
                    tokens.Add(new Token("op", oneCharOp, line_num, col_num));
                    i++; col_num++;
                    continue;
                }

                // Punctuation/Special
                if (Special_Reg.IsMatch(c.ToString()))
                {
                    Console.WriteLine($"< punc, {c} >");
                    tokens.Add(new Token("punc", c.ToString(), line_num, col_num));
                    i++; col_num++;
                    continue;
                }

                // Unknown character
                Console.WriteLine($"ERROR: Unknown character '{c}' at line {line_num}, column {col_num}");
                tokens.Add(new Token("error", c.ToString(), line_num, col_num));
                i++; col_num++;
            }
        }

        // --- PARSER ---
        static int currentToken = 0;

        static Token Peek() => currentToken < tokens.Count ? tokens[currentToken] : null;

        static Token Next()
        {
            if (currentToken < tokens.Count) return tokens[currentToken++];
            return null;
        }

        static void Expect(string type, string value = null, string expected = null)
        {
            var t = Peek();
            if (t == null || t.Type != type || (value != null && t.Value != value))
            {
                string found = t == null ? "EOF" : t.Value;
                string errMsg = $"ERROR: {found} at line {(t?.Line ?? tokens[tokens.Count - 1].Line)}, column {(t?.Column ?? tokens[tokens.Count - 1].Column)}; Expected {expected ?? type.ToUpper()}";
                Console.WriteLine(errMsg);
                throw new Exception(); // stop further parsing after first error
            }
            Next();
        }

        static void ParseTokens()
        {
            currentToken = 0;
            try
            {
                while (currentToken < tokens.Count)
                {
                    ParseStatement();
                }
            }
            catch
            {
                // Stop after first syntax error
            }
        }

        static void ParseStatement()
        {
            var t = Peek();
            if (t == null) return;

            if (t.Type == "keyword")
            {
                switch (t.Value)
                {
                    case "int":
                    case "float":
                        Next();
                        Expect("id", null, "IDENTIFIER");
                        Expect("op", "=", "EQUALS SIGN");
                        ParseExpression();
                        Expect("punc", ";", "SEMICOLON");
                        break;

                    case "if":
                        Next();
                        Expect("punc", "(", "OPEN PARENTHESIS");
                        ParseExpression(); // condition simplified to expression parse
                        Expect("punc", ")", "CLOSE PARENTHESIS");
                        ParseBlock();
                        // Optional else block
                        if (Peek() != null && Peek().Type == "keyword" && Peek().Value == "else")
                        {
                            Next();
                            ParseBlock();
                        }
                        break;

                    default:
                        Console.WriteLine($"ERROR: Unexpected keyword '{t.Value}' at line {t.Line}, column {t.Column}");
                        throw new Exception();
                }
            }
            else if (t.Type == "id")
            {
                Next();
                if (Peek() != null && Peek().Type == "op" && Peek().Value == "=")
                {
                    Next();
                    ParseExpression();
                    Expect("punc", ";", "SEMICOLON");
                }
                else
                {
                    var next = Peek();
                    string errMsg = $"ERROR: {next?.Value ?? "EOF"} at line {next?.Line ?? t.Line}, column {next?.Column ?? t.Column}; Expected '='";
                    Console.WriteLine(errMsg);
                    throw new Exception();
                }
            }
            else
            {
                string errMsg = $"ERROR: Unexpected token '{t.Value}' at line {t.Line}, column {t.Column}";
                Console.WriteLine(errMsg);
                throw new Exception();
            }
        }

        static void ParseBlock()
        {
            Expect("punc", "{", "OPEN BRACE");
            while (Peek() != null && !(Peek().Type == "punc" && Peek().Value == "}"))
            {
                ParseStatement();
            }
            Expect("punc", "}", "CLOSE BRACE");
        }

        static void ParseExpression()
        {
            var t = Peek();
            if (t != null && (t.Type == "id" || t.Type == "digit"))
            {
                Next();
                if (Peek() != null && Peek().Type == "op")
                {
                    Next();
                    ParseExpression();
                }
            }
            else
            {
                string errMsg = $"ERROR: {t?.Value ?? "EOF"} at line {t?.Line ?? tokens[tokens.Count - 1].Line}, column {t?.Column ?? tokens[tokens.Count - 1].Column}; Expected EXPRESSION";
                Console.WriteLine(errMsg);
                throw new Exception();
            }
        }
    }
}
