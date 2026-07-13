using Bookify.Web.Core.Consts;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Web.Core.ViewModel
{
    public class BookCopyForm
    {
        public int Id { get; set; }

        public int BookId { get; set; }
        public bool IsAvailableForRental { get; set; }
        [Display(Name = "Edition Number")]
        [Range(1,1000,ErrorMessage =Errors.NotAllowedRange)]
        public int EditionNumber { get; set; }

        public bool ShowRentalInput { get; set; }
    }
}
