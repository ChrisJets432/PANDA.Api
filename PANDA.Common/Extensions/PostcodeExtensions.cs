using System.Text.RegularExpressions;
using PANDA.Common.Resources;

namespace PANDA.Common.Extensions;

public static class PostcodeExtensions
{
    public const string Pattern = @"^([A-Z]{1,2}\d[A-Z\d]?\s*\d[A-Z]{2})$";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryParse(string? postcode, out string? result)
    {
        result = Normalise(postcode);
        return IsValid(result);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <returns></returns>
    public static bool IsValid(string? postcode)
    {
        return !string.IsNullOrWhiteSpace(postcode) && Regex.IsMatch(postcode, Pattern, RegexOptions.IgnoreCase);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <returns></returns>
    public static string? Normalise(string? postcode)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            return null;
        }

        postcode = postcode.ToUpperInvariant().Trim();
        postcode = Regex.Replace(postcode, @"-", string.Empty); // Remove all spaces initially
        postcode = Regex.Replace(postcode, @"\s+", string.Empty); // Remove all spaces initially

        ArgumentOutOfRangeException.ThrowIfLessThan(postcode.Length, 5, "Postcode must be greater than 5 characters");
        
        return $"{postcode[..^3]} {postcode[^3..]}";
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="postcode"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string? Coerce(string? postcode)
    {
        var normalised = Normalise(postcode);

        if (!IsValid(normalised))
        {
            throw new ArgumentException(Localisation.Error_InvalidPostcode);
        }

        return normalised;
    }
}