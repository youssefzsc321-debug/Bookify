namespace Bookify.Web.Core.Consts
{
    public class Errors
    {
        public const string MaxLength = "Length Cannot be more than {1} Characters";
        public const string Duplicated = "{0} with the same name is already exists!";
        public const string NotAllowedExtention = "Only .png , .jpg , .jepg Files are allowed!";
        public const string MaxSize = "File Cannot be more than 2MB!";
        public const string TitleWithTheSameAuthor = "Title With The Same Author is alerady exist";
        public const string AuthorWithTheSameTitle = "Author With The Same Title is alerady exist";
        public const string NotAllowFuteruData = "Date cannot be in the future!";
        public const string NotAllowedRange = "The values must between {1} and {2}";

    }
}
