using Bookify.Web.Core.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bookify.Web.Core.ViewModel
{
    public class BookCopyVM
    {
        public int Id { get; set; }

        public string? BookTilte { get; set; }

        public bool IsAvailableForRental { get; set; }
        public int EditionNumber { get; set; }
        public int SerialNumber { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
