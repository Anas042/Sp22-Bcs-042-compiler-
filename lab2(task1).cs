using System;
using System.Text.RegularExpressions;

class LogicalTokenParser
{
    static void Main()
    {
        // Regex pattern to identify logical operators and parentheses
        string logicPattern = @"\s*(\&\&|\|\||!|\(|\))\s*";

        // Input string to test against
        string expression = "x && y || !z (x || y)";

        // Compile regex
        var logicRegex = new Regex(logicPattern);

        // Extract matching tokens
        var tokenMatches = logicRegex.Matches(expression);

        // Display each token found
        foreach (var match in tokenMatches)
        {
            Console.WriteLine($"Token: {(match as Match).Value}");
        }
    }
}
