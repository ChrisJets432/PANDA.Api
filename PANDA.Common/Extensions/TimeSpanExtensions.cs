using System.Text.RegularExpressions;
using PANDA.Common.Resources;

namespace PANDA.Common.Extensions;

public static class TimeSpanExtensions
{
    public static bool TryParse(string durationString, out TimeSpan? duration)
    {
        duration = null;
        
        if (string.IsNullOrWhiteSpace(durationString))
        {
            return false;
        }
        
        var converted = ParseDuration(durationString);

        if (converted.Ticks == 0)
        {
            return false;
        }

        duration = converted;
        return true;
    }
    
    public static TimeSpan ParseDuration(string durationString)
    {
        var hours = 0;
        var minutes = 0;
        var seconds = 0;
        var parts = durationString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            if (part.EndsWith(Localisation.Input_Hour, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(part.Replace(Localisation.Input_Hour, "", StringComparison.OrdinalIgnoreCase), out var hr))
                {
                    hours = hr;
                }
            }
            else if (part.EndsWith(Localisation.Input_Minute, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(part.Replace(Localisation.Input_Minute, "", StringComparison.OrdinalIgnoreCase), out var min))
                {
                    minutes = min;
                }
            }
            else if (part.EndsWith(Localisation.Input_Second, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(part.Replace(Localisation.Input_Second, "", StringComparison.OrdinalIgnoreCase), out var sec))
                {
                    seconds = sec;
                }
            }
            else
            {
                throw new NotSupportedException($"{Regex.Replace(part , @"/d+", "/")} not supported unit");
            }
        }

        return new TimeSpan(hours, minutes, seconds);
    }
    
    public static string StringDuration(this TimeSpan timeSpan)
    {
        var formatted = "";

        if (timeSpan.Hours > 0)
        {
            formatted += $"{timeSpan.Hours}hr ";
        }

        if (timeSpan.Minutes > 0 || timeSpan.Hours > 0)
        {
            formatted += $"{timeSpan.Minutes}m";
        }

        if (timeSpan.Seconds > 0) {
            formatted += $"{timeSpan.Seconds}s";
        }

        return formatted.Trim();
    }
}