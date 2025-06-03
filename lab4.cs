using System;
using System.Text.RegularExpressions;

class Program
{
    static string[] keywords = { "int", "if", "else" };

    static void Main()
    {
        Console.WriteLine("Enter some code:");
        string input = Console.ReadLine();

        Tokenize(input);
    }

    static void Tokenize(string code)
    {
        // Matches words, numbers, and symbols
        string pattern = @"\w+|[^\s\w]";
        MatchCollection tokens = Regex.Matches(code, pattern);

        foreach (Match token in tokens)
        {
            string value = token.Value;

            if (IsKeyword(value))
                Console.WriteLine($"[Keyword]     {value}");
            else if (Regex.IsMatch(value, @"^\d+$"))
                Console.WriteLine($"[Number]      {value}");
            else if (Regex.IsMatch(value, @"^[a-zA-Z_]\w*$"))
                Console.WriteLine($"[Identifier]  {value}");
            else
                Console.WriteLine($"[Operator]    {value}");
        }
    }

    static bool IsKeyword(string token)
    {
        foreach (string key in keywords)
        {
            if (token == key)
                return true;
        }
        return false;
    }
}
