namespace Bookify.Web.Core.Consts
{
    public static class Errors
    {
        public const string MaxLengthCharcters = "Length Cannot be more than {1} Characters";
        public const string MaxLengthDigit = "Length Cannot be more than {1} Characters";
        public const string MinLengthDigit = "Length Cannot be less than {1} Characters";

        public const string Duplicated = "This {0} is already exists!";
        public const string NotAllowedExtention = "Only .png , .jpg , .jepg Files are allowed!";
        public const string MaxSize = "File Cannot be more than 2MB!";
        public const string TitleWithTheSameAuthor = "Title With The Same Author is alerady exist";
        public const string AuthorWithTheSameTitle = "Author With The Same Title is alerady exist";
        public const string NotAllowFuteruData = "Date cannot be in the future!";
        public const string NotAllowedRange = "The values must between {1} and {2}";
        public const string NotAllowedPassword = "The {0} must be at least {2} and at max {1} characters long.";
        public const string PassworNotMatch = "The password and confirmation password do not match.";
        public const string PasswordNotMatchCritera = "Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, and one number.";
        
        public const string UserNamePattern = "Username can only contain letters, numbers, and characters like: -, ., _, @, +, #";
        public const string JustEnglistLetters = "Username must contain only English letters and spaces. Numbers and symbols are not allowed.";
        public const string NotisTheCuurnetPassword = "The password you entered doesn't match your current password.";
        public const string Required = "The {0} field is required.";
        public const string NotAllowedPhone = "Please enter a valid Egyptian mobile number (11 digits starting with 010, 011, 012, or 015).";

        public const string NotAllowedNationlaId = "Invalid Egyptian National ID.";

    }
}
