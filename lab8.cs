using System;

class DFA_CVariable
{
    static void Main()
    {
        Console.WriteLine("Enter a variable name to check:");
        string input = Console.ReadLine();

        if (IsValidVariable(input))
            Console.WriteLine("✅ Valid C variable name.");
        else
            Console.WriteLine("❌ Invalid C variable name.");
    }

    static bool IsValidVariable(string input)
    {
        int state = 0;

        for (int i = 0; i < input.Length; i++)
        {
            char ch = input[i];

            switch (state)
            {
                case 0:
                    if (char.IsLetter(ch) || ch == '_')
                        state = 1;
                    else
                        return false;
                    break;

                case 1:
                    if (char.IsLetterOrDigit(ch) || ch == '_')
                        state = 1;
                    else
                        return false;
                    break;
            }
        }

        return state == 1;
    }
}
