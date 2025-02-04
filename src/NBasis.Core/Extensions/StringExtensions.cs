namespace NBasis;

public static class StringExtensions
{
    /// <summary>
    /// Check if a string is null or empty
    /// </summary>
    public static bool IsNullOrEmpty(this string input)
    {
        return String.IsNullOrEmpty(input);
    }

    /// <summary>
    /// Check if a string is null or whitespace
    /// </summary>
    public static bool IsNullOrWhiteSpace(this string input)
    {
        return String.IsNullOrWhiteSpace(input);
    }

    public static string FormatWith(this string format, params object[] args)
    {
        return string.Format(format, args);
    }

    /// <summary>
    /// Trim and convert to lower case
    /// </summary>
    public static string TrimAndLower(this String input)
    {
        if (String.IsNullOrEmpty(input)) return input;
        return input.Trim().ToLower();
    }

    /// <summary>
    /// Make the first character upper case
    /// </summary>
    public static string Capitalize(this string input)
    {
        if (String.IsNullOrEmpty(input)) return input;
        if (input.Length == 1) return input.ToUpper();
        return char.ToUpper(input[0]) + input[1..];
    }
}