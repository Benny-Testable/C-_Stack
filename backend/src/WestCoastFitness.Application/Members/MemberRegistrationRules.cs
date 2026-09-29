namespace WestCoastFitness.Application;

public static class MemberRegistrationRules
{
    public static string? Validate(string email, string fullName, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@', StringComparison.Ordinal) || email.Length > 256)
        {
            return "Enter a valid email address.";
        }

        if (string.IsNullOrWhiteSpace(fullName) || fullName.Trim().Length < 2)
        {
            return "Enter the member's full name.";
        }

        if (string.IsNullOrEmpty(password) || password.Length < 10)
        {
            return "Password must be at least 10 characters.";
        }

        var hasUpper = false;
        var hasLower = false;
        var hasDigit = false;
        foreach (var ch in password)
        {
            if (char.IsUpper(ch))
            {
                hasUpper = true;
            }
            else if (char.IsLower(ch))
            {
                hasLower = true;
            }
            else if (char.IsDigit(ch))
            {
                hasDigit = true;
            }
        }

        if (!hasUpper || !hasLower || !hasDigit)
        {
            return "Password must include an uppercase letter, a lowercase letter, and a digit.";
        }

        return null;
    }
}
