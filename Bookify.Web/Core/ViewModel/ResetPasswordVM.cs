using Bookify.Web.Core.Consts;

namespace Bookify.Web.Core.ViewModel
{
    public class ResetPasswordVM
    {

        public string Id { get; set; }
        

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [RegularExpression(RegexPatterns.PasswordPattern, ErrorMessage = Errors.PasswordNotMatchCritera)]
        public string NewPassword { get; set; }


        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("NewPassword", ErrorMessage = Errors.PassworNotMatch)] 
        public string ConfirmPassword { get; set; }
    }
}
