using System;
using System.Text;
using System.Text.RegularExpressions;

class PasswordGeneratorApp
{
    static void Main(string[] args)
    {
        // Gather user inputs
        Console.Write("First Name: ");
        string fName = Console.ReadLine();

        Console.Write("Last Name: ");
        string lName = Console.ReadLine();

        Console.Write("Registration #: ");
        string regID = Console.ReadLine();

        Console.Write("Favorite Food: ");
        string favFood = Console.ReadLine();

        Console.Write("Favorite Game: ");
        string favGame = Console.ReadLine();

        // Call function to create password
        string finalPassword = CreateSecurePassword(fName, lName, regID, favFood, favGame);

        Console.WriteLine("Generated Password: " + finalPassword);
    }

    static string CreateSecurePassword(string first, string last, string reg, string food, string game)
    {
        // Merge inputs
        string combinedInput = string.Concat(first, last, reg, food, game);

        // Remove non-alphanumeric characters
        string cleaned = Regex.Replace(combinedInput, "[^a-zA-Z0-9]", "");

        // Add complexity
        string specialSymbols = "!@#$%^&*()_+[]{}|;:,.<>?/~`";
        StringBuilder passwordBuilder = new StringBuilder(cleaned);
        Random rng = new Random();

        for (int i = 0; i < 4; i++)
        {
            passwordBuilder.Append(rng.Next(0, 10)); // random digit
            passwordBuilder.Append(specialSymbols[rng.Next(specialSymbols.Length)]); // random symbol
        }

        // Ensure minimum length
        while (passwordBuilder.Length < 12)
        {
            passwordBuilder.Insert(0, 'Z');
        }

        // Shuffle final string
        string rawPassword = passwordBuilder.ToString();
        StringBuilder shuffled = new StringBuilder();
        while (rawPassword.Length > 0)
        {
            int idx = rng.Next(rawPassword.Length);
            shuffled.Append(rawPassword[idx]);
            rawPassword = rawPassword.Remove(idx, 1);
        }

        return shuffled.ToString();
    }
}
