using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;

namespace Bookify.Web.Core.ViewModel
{
    public class BookFormVM
    {
        public int Id { get; set; }
        [MaxLength(500, ErrorMessage = Errors.MaxLength)]
        public string Title { get; set; }



        [Display(Name = "Author")]
        public int AuthorsId { get; set; }
        public IEnumerable<SelectListItem>? Authors { get; set; }

        [MaxLength(200, ErrorMessage = Errors.MaxLength)]
        public string Publisher { get; set; }

        [Display(Name = "Publishing Date")]
        public DateTime PublishingDate { get; set; }

        public IFormFile? Image { get; set; }
        public string? ImageUrl { get; set; }

        [MaxLength(50, ErrorMessage = Errors.MaxLength)]
        public string Hall { get; set; }

        [Display(Name = "Is available for rental?")]
        public bool IsAvailableForRental { get; set; }

        public string Description { get; set; }


        public IList<int> SelectedCategories { get; set; } = new List<int>();
        public IEnumerable<SelectListItem>? Categories { get; set; }



    }
}
