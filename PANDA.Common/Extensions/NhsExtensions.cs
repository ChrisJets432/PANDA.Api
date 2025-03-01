namespace PANDA.Common.Extensions;

public static class NhsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="cleanedNhsNumber"></param>
    /// <returns></returns>
    public static bool TryParse(string? input, out string? cleanedNhsNumber)
    {
        cleanedNhsNumber = null;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var nhsNumber = input.Replace(" ", string.Empty);

        if (nhsNumber.Length != 10 || !nhsNumber.All(char.IsDigit))
        {
            return false;
        }

        if (!NumberChecksumIsValid(nhsNumber))
        {
            return false;
        }

        cleanedNhsNumber = nhsNumber;
        return true;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nhsNumber"></param>
    /// <returns></returns>
    public static bool IsValid(string? nhsNumber)
    {
        return TryParse(nhsNumber, out _);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="nhsNumber"></param>
    /// <returns></returns>
    public static bool NumberChecksumIsValid(string nhsNumber)
    {
        int[] weights = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        var sum = 0;

        for (var i = 0; i < 9; i++)
        {
            sum += (nhsNumber[i] - '0') * weights[i];
        }

        var calculatedCheckDigit = (11 - (sum % 11)) % 11;
        var providedCheckDigit = nhsNumber[9] - '0';

        return calculatedCheckDigit == providedCheckDigit;
    }
}