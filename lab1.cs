using System;
using System.Text.RegularExpressions;

public class PasswordValidator
{
    public static void Main()
    {
        // Example password to be checked
        string inputPwd = "Sp22-bcs-036";

        // Regex pattern to meet the specified criteria
        string regexPattern = 
            @"^(?=(.*\d.*){2})(?=.*[A-Z])(?=(.*[a-z]){4})(?=(.*[!@#$%^&*(),.?\""{}`|<>]){2}).{1,12}$";

        // Validate the password format using regular expression
        bool isValid = Regex.IsMatch(inputPwd, regexPattern);

        Console.WriteLine(isValid ? "Password is valid." : "Password is invalid.");
    }
}
