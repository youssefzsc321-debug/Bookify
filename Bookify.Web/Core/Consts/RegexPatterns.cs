namespace Bookify.Web.Core.Consts
{
    public static class RegexPatterns
    {
        public const string PasswordPattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\W_]{8,}$";
        public const string UserNamePattern = @"^[a-zA-Z0-9-._@+#]+$";
        public const string AllowJustEnglish = @"^[a-zA-Z\s]+$";
        public const string AllowPhone = @"^01[0125][0-9]{8}$";
    }
}
