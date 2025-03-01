using Microsoft.Extensions.Configuration;

namespace PANDA.Common
{
    public static class ConfigurationManager
    {
        private static IConfiguration AppSettings { get; }
        
        static ConfigurationManager()
        {
            AppSettings = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
        }

        public static object? GetValue(string identify)
        {
            var seperated = identify.Split(':');
            var sections = seperated.Take(seperated.Length - 1);
            var section = AppSettings;

            var index = 0;
            while (index < sections.Count())
            {
                var current = sections.ElementAtOrDefault(index);
                section = !string.IsNullOrEmpty(current) ? section?.GetSection(current) : null;
                index++;
            }

            return section?[seperated.Last()];
        }

        public static T? GetValue<T>(string identify)
        {
            return (T)(GetValue(identify) ?? default)!;
        }

        public static string? ByPath(string path)
        {
            return AppSettings[path];
        }
    }
}