using System;

class RecursiveParser
{
    static string expression;
    static int pos = 0;

    static void Main()
    {
        Console.WriteLine("Please input an arithmetic expression:");
        expression = Console.ReadLine();
        expression = expression.Replace(" ", ""); // strip spaces

        try
        {
            ParseExpression();
            if (pos == expression.Length)
            {
                Console.WriteLine("Expression is valid!");
            }
            else
            {
                Console.WriteLine("Expression is invalid!");
            }
        }
        catch
        {
            Console.WriteLine("Expression is invalid!");
        }
    }

    // E -> T E_
    static void ParseExpression()
    {
        ParseTerm();
        ParseExpressionPrime();
    }

    // E' -> + T E' | ε
    static void ParseExpressionPrime()
    {
        if (Consume('+'))
        {
            ParseTerm();
            ParseExpressionPrime();
        }
    }

    // T -> F T_
    static void ParseTerm()
    {
        ParseFactor();
        ParseTermPrime();
    }

    // T' -> * F T' | ε
    static void ParseTermPrime()
    {
        if (Consume('*'))
        {
            ParseFactor();
            ParseTermPrime();
        }
    }

    // F -> (E) | number
    static void ParseFactor()
    {
        if (Consume('('))
        {
            ParseExpression();
            if (!Consume(')'))
                throw new Exception("Expected closing parenthesis");
        }
        else if (char.IsDigit(Current()))
        {
            while (char.IsDigit(Current()))
                pos++;
        }
        else
        {
            throw new Exception("Unexpected character in factor");
        }
    }

    static bool Consume(char ch)
    {
        if (pos < expression.Length && expression[pos] == ch)
        {
            pos++;
            return true;
        }
        return false;
    }

    static char Current()
    {
        return pos < expression.Length ? expression[pos] : '\0';
    }
}
