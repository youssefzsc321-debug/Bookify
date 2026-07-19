using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel
{
    public class UserFormVM
    {
        public string? Id { get; set; }

        [MaxLength(100,ErrorMessage =Errors.MaxLength)]
        [Display(Name ="Full Name")]
        [RegularExpression(RegexPatterns.AllowJustEnglish,ErrorMessage =Errors.JustEnglistLetters)]
        public string FullName { get; set; }

        [MaxLength(50,ErrorMessage =Errors.MaxLength)]
        [Display(Name ="User Name")]
        [Remote(action: "AllowUserName", controller: "User",AdditionalFields ="Id",ErrorMessage =Errors.DublicatedUserName)]
        [RegularExpression(RegexPatterns.UserNamePattern,ErrorMessage =Errors.UserNamePattern)]
        public string UserName { get; set; }

        
        [MaxLength(150,ErrorMessage =Errors.MaxLength)]
        [EmailAddress]
        [Remote(action: "AllowUserEmail", controller: "User", AdditionalFields ="Id",ErrorMessage =Errors.DublicatedEmail)]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage =Errors.NotAllowedPassword , MinimumLength = 8)]
     
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(RegexPatterns.PasswordPattern,ErrorMessage =Errors.PasswordNotMatchCritera)]
        [RequiredIf("Id==null", ErrorMessage = Errors.Required)]
        public string? Password { get; set; }

       
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage =Errors.PassworNotMatch )] 
        [RequiredIf("Id==null", ErrorMessage = Errors.Required)]
        public string? ConfirmPassword { get; set; }

        public IList<string> SelectedRoles { get; set; }
        public IEnumerable<SelectListItem>? Roles { get; set; }


    }
}
