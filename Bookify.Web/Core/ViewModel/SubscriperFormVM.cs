using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel
{

    
    public class SubscriperFormVM
    {
        public string? Key { get; set; }

        [MaxLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [MaxLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [AssertThat("DateOfBirth <=Today()", ErrorMessage = Errors.NotAllowFuteruData)]
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(20,ErrorMessage =Errors.MaxLengthDigit)]
        [MinLength(14,ErrorMessage =Errors.MinLengthDigit)]
        [Remote(action: "AllowNationalId",controller: "Subscripers",AdditionalFields = "Key", ErrorMessage =Errors.Duplicated)]
        [RegularExpression(RegexPatterns.AllowNationalId,ErrorMessage =Errors.NotAllowedNationlaId)]
        [Display(Name = "National Id")]
        public string NationalId { get; set; }
        

        [Remote(action: "AllowMobile", controller: "Subscripers",AdditionalFields = "Key", ErrorMessage =Errors.Duplicated)]
        [RegularExpression(RegexPatterns.AllowPhone,ErrorMessage =Errors.NotAllowedPhone)]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [MaxLength(150)]
        [Remote(action: "AllowEmail", controller: "Subscripers",AdditionalFields = "Key", ErrorMessage =Errors.Duplicated)]
        [EmailAddress]
        public string Email { get; set; }

        public bool HasWhatsApp { get; set; }

        [RequiredIf("Key==''", ErrorMessage = Errors.Required)]
        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }
        public string? imageThumbnailUrl { get; set; }


        [Display(Name = "Area")]
        public int SelectedArea { get; set; }
        public IEnumerable<SelectListItem>? Areas { get; set; }

        [Display(Name = "Governorate")]
        public int SelectedGovernorate { get; set; }
        public IEnumerable<SelectListItem>? Governorates { get; set; }
        
        [MaxLength(500)]
        public string Address { get; set; }

    }
}
