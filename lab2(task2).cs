using System;
using System.Text.RegularExpressions;

class RelationalOperatorExtractor
{
    static void Main()
    {
        // Define regex to capture relational operators
        string operatorPattern = @"\s*(==|!=|>=|<=|\>|\<)\s*";

        // Sample code string containing relational expressions
        string expressionText = "a == b && c != d || e >= f && g < h";

        // Initialize Regex with the pattern
        var opRegex = new Regex(operatorPattern);

        // Retrieve all matched relational operators
        var foundOperators = opRegex.Matches(expressionText);

        // Display results
        foreach (Match op in foundOperators)
        {
            Console.WriteLine($"Operator detected: '{op.Value.Trim()}'");
        }
    }
}
