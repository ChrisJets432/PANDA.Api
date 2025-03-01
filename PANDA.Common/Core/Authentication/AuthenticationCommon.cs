namespace PANDA.Common.Core.Authentication;

public static class AuthenticationCommon
{
    public static class Static
    {
        public static class Headers
        {
            public const string Authorisation = "Authorization";
            public const string Culture = $"{nameof(PANDA)}-Culture";
        }

        public const string AuthHeader = "Authorization";
        public const string Token = "Token";

        public static class Schemes
        {
            public const string Basic = "Basic";
            public const string Bearer = "Bearer";
        }

        public static class Claims
        {
            private const string Host = $"{nameof(PANDA)}://";
            public const string Property = $"{Host}property";
            public const string PropertyByKey = $"{Property}/{{0}}";
            public const string Culture = $"{Host}culture";
        }
    }

    public static class Cultures
    {
        public const string English = "en";
        public const string French = "fr";
    }

    public const string SchemeName = $"{nameof(PANDA)}Authentication";
}