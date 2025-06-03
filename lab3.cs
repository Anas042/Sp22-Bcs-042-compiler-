using System;
using System.Text.RegularExpressions;

class FloatValidator
{
    static void Main()
    {
        // Pattern matches numbers with optional sign, up to 3 digits before/after decimal
        string floatPattern = @"^[\+\-]?\d{1,3}(\.\d{1,3})?$|^[\+\-]?\.\d{1,3}$";

        // Array of test inputs
        string[] samples = {
            "123",       // valid
            "-12.34",    // valid
            "+0.567",    // valid
            ".678",      // valid
            "0.5",       // valid
            "123456",    // invalid
            "1.2345",    // invalid
            "+1234",     // invalid
            ".1234"      // invalid
        };

        // Evaluate each sample
        foreach (var item in samples)
        {
            var valid = Regex.IsMatch(item, floatPattern);
            Console.WriteLine($"Input '{item}' → {(valid ? "Valid" : "Invalid")}");
        }
    }
}
