using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SemanticAnalyzerLab
{
    class Program
    {
        static List<List<string>> Symboltable = new List<List<string>>();
        static List<string> finalArray = new List<string>();
        static List<int> Constants = new List<int>();
        static Regex variable_Reg = new Regex(@"^[A-Za-z_][A-Za-z0-9]*$");
        static bool if_deleted = false;

        static void Main(string[] args)
        {
            InitializeSymbolTable();
            InitializeFinalArray();
            PrintLexerOutput();

            for (int i = 0; i < finalArray.Count; i++)
            {
                Semantic_Analysis(i);
            }

            Console.WriteLine("\nSemantic Analysis Completed.");
            Console.ReadLine();
        }

        static void InitializeSymbolTable()
        {
            Symboltable.Add(new List<string> { "x", "id", "int", "0" });
            Symboltable.Add(new List<string> { "y", "id", "int", "0" });
            Symboltable.Add(new List<string> { "i", "id", "int", "0" });
            Symboltable.Add(new List<string> { "l", "id", "char", "0" });
        }

        static void InitializeFinalArray()
        {
            finalArray.AddRange(new string[] {
                "int", "main", "(", ")", "{",
                "int", "x", ";",
                "x", ";",
                "x", "=", "2", "+", "5", "+", "(", "4", "*", "8", ")", "+", "l", "/", "9.0", ";",
                "if", "(", "x", "+", "y", ")", "{",
                "if", "(", "x", "!=", "4", ")", "{",
                "x", "=", "6", ";",
                "y", "=", "10", ";",
                "i", "=", "11", ";",
                "}", "}", 
                "}"
            });
        }

        static void PrintLexerOutput()
        {
            Console.WriteLine("Tokenizing input...");

            int row = 1, col = 1;
            foreach (string token in finalArray)
            {
                if (token == "int")
                    Console.WriteLine($"INT ({row},{col})");
                else if (token == "main")
                    Console.WriteLine($"MAIN ({row},{col})");
                else if (token == "(")
                    Console.WriteLine($"LPAREN ({row},{col})");
                else if (token == ")")
                    Console.WriteLine($"RPAREN ({row},{col})");
                else if (token == "{")
                    Console.WriteLine($"LBRACE ({row},{col})");
                else if (token == "}")
                    Console.WriteLine($"RBRACE ({row},{col})");
                else if (token == ";")
                    Console.WriteLine($"SEMI ({row},{col})");
                else if (token == "=")
                    Console.WriteLine($"ASSIGN ({row},{col})");
                else if (token == "+")
                    Console.WriteLine($"PLUS ({row},{col})");
                else if (token == "-")
                    Console.WriteLine($"MINUS ({row},{col})");
                else if (token == "*")
                    Console.WriteLine($"TIMES ({row},{col})");
                else if (token == "/")
                    Console.WriteLine($"DIV ({row},{col})");
                else if (token == "!=")
                    Console.WriteLine($"NEQ ({row},{col})");
                else if (token == "if")
                    Console.WriteLine($"IF ({row},{col})");
                else if (token == "else")
                    Console.WriteLine($"ELSE ({row},{col})");
                else if (Regex.IsMatch(token, @"^[0-9]+$"))
                    Console.WriteLine($"INT_CONST ({row},{col}): {token}");
                else if (Regex.IsMatch(token, @"^[0-9]+\.[0-9]+$"))
                    Console.WriteLine($"FLOAT_CONST ({row},{col}): {token}");
                else if (Regex.IsMatch(token, @"^[a-zA-Z]$"))
                    Console.WriteLine($"CHAR_CONST ({row},{col}): {token}");
                else if (variable_Reg.Match(token).Success)
                    Console.WriteLine($"ID ({row},{col}): {token}");
                else
                    Console.WriteLine($"UNKNOWN ({row},{col}): {token}");

                col += token.Length + 1;
                if (token == ";") row++;
            }
            Console.WriteLine($"EOF ({row},{col})");
        }

        static void Semantic_Analysis(int k)
        {
            if (k <= 0 || k >= finalArray.Count - 1) return;

            // Handle addition or subtraction between variables
            if (finalArray[k] == "+" || finalArray[k] == "-")
            {
                if (variable_Reg.Match(finalArray[k - 1]).Success && variable_Reg.Match(finalArray[k + 1]).Success)
                {
                    // find type from symbol table for left operand
                    int leftSymbolIndex = FindSymbol(finalArray[k - 1]);
                    int rightSymbolIndex = FindSymbol(finalArray[k + 1]);
                    int assignSymbolIndex = FindSymbol(finalArray[k - 3]); // variable on LHS of assignment

                    if (leftSymbolIndex == -1 || rightSymbolIndex == -1 || assignSymbolIndex == -1)
                        return;

                    string type = Symboltable[assignSymbolIndex][2];

                    if (type == Symboltable[leftSymbolIndex][2] && type == Symboltable[rightSymbolIndex][2])
                    {
                        // Calculate sum of constant values
                        int ans = Convert.ToInt32(Symboltable[leftSymbolIndex][3]) +
                                  Convert.ToInt32(Symboltable[rightSymbolIndex][3]);

                        Constants.Add(ans);

                        // Update symbol table for assigned variable
                        Symboltable[assignSymbolIndex][3] = ans.ToString();

                        if (Constants.Count > 0)
                            Constants.RemoveAt(Constants.Count - 1);
                        Constants.Add(ans);
                    }
                }
            }

            // Handle '>' condition in if statement (simplified)
            if (finalArray[k] == ">")
            {
                if (variable_Reg.Match(finalArray[k - 1]).Success && variable_Reg.Match(finalArray[k + 1]).Success)
                {
                    int before_i = FindSymbol(finalArray[k - 1]);
                    int after_i = FindSymbol(finalArray[k + 1]);

                    if (before_i == -1 || after_i == -1)
                        return;

                    if (Convert.ToInt32(Symboltable[before_i][3]) > Convert.ToInt32(Symboltable[after_i][3]))
                    {
                        RemoveElseBlock();
                    }
                    else
                    {
                        RemoveIfBlock();
                        if_deleted = true;
                    }
                }
            }
        }

        static int FindSymbol(string name)
        {
            for (int i = 0; i < Symboltable.Count; i++)
            {
                if (Symboltable[i][0] == name)
                    return i;
            }
            return -1;
        }

        static void RemoveElseBlock()
        {
            int start_of_else = finalArray.IndexOf("else");
            if (start_of_else == -1) return;

            int braceCount = 0;
            int end_of_else = -1;
            for (int i = start_of_else; i < finalArray.Count; i++)
            {
                if (finalArray[i] == "{") braceCount++;
                else if (finalArray[i] == "}") braceCount--;

                if (braceCount == 0 && i > start_of_else)
                {
                    end_of_else = i;
                    break;
                }
            }

            if (end_of_else == -1) return;

            finalArray.RemoveRange(start_of_else, end_of_else - start_of_else + 1);
        }

        static void RemoveIfBlock()
        {
            int start_of_if = finalArray.IndexOf("if");
            if (start_of_if == -1) return;

            int braceCount = 0;
            int end_of_if = -1;
            for (int i = start_of_if; i < finalArray.Count; i++)
            {
                if (finalArray[i] == "{") braceCount++;
                else if (finalArray[i] == "}") braceCount--;

                if (braceCount == 0 && i > start_of_if)
                {
                    end_of_if = i;
                    break;
                }
            }

            if (end_of_if == -1) return;

            finalArray.RemoveRange(start_of_if, end_of_if - start_of_if + 1);
        }
    }
}
