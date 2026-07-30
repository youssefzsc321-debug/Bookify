namespace Bookify.Web.Core.Consts
{
    public static class RegexPatterns
    {
        public const string PasswordPattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[a-zA-Z\\d\\W_]{8,}$";
        public const string UserNamePattern = @"^[a-zA-Z0-9-._@+#]+$";
        public const string AllowJustEnglish = @"^[a-zA-Z\s]+$";
        public const string AllowPhone = @"^01[0125][0-9]{8}$";
        public const string AllowNationalId = @"^(2|3)[0-9]{2}(0[1-9]|1[0-2])(0[1-9]|[12][0-9]|3[01])(01|02|03|04|11|12|13|14|15|16|17|18|19|21|22|23|24|25|26|27|28|29|31|32|33|34|35|88)[0-9]{5}$";
    }
}
