using System;

class GrammarParser
{
    static string input;
    static int position = 0;

    static void Main()
    {
        Console.WriteLine("Enter statement (e.g., if(id<num){id=5+3;}else{id=2+1;}):");
        input = Console.ReadLine();
        input = input.Replace(" ", ""); // Remove spaces for easier parsing

        try
        {
            ParseStatement();

            if (position == input.Length)
                Console.WriteLine("Valid Syntax!");
            else
                Console.WriteLine("Invalid Syntax!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Invalid Syntax!");
        }
    }

    // S → if(C){S}else{S} | id=E;
    static void ParseStatement()
    {
        if (Match("if"))
        {
            Match("(");
            ParseCondition();
            Match(")");
            Match("{");
            ParseStatement();
            Match("}");
            Match("else");
            Match("{");
            ParseStatement();
            Match("}");
        }
        else if (Match("id"))
        {
            Match("=");
            ParseExpression();
            Match(";");
        }
        else
        {
            throw new Exception("Invalid statement");
        }
    }

    // C → id<num
    static void ParseCondition()
    {
        Match("id");
        Match("<");
        Match("num");
    }

    // E → T + T
    static void ParseExpression()
    {
        ParseTerm();
        Match("+");
        ParseTerm();
    }

    // T → id | num
    static void ParseTerm()
    {
        if (!(Match("id") || Match("num")))
            throw new Exception("Expected id or num");
    }

    // Helper: matches the next token if it exists
    static bool Match(string token)
    {
        if (input.Substring(position).StartsWith(token))
        {
            position += token.Length;
            return true;
        }
        return false;
    }
}
