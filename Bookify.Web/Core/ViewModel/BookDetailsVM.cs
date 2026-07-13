using Bookify.Web.Core.Models;

namespace Bookify.Web.Core.ViewModel
{
    public class BookDetailsVM
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
        public string  AuthorsName { get; set; }

        public string Publisher { get; set; }
        public DateTime PublishingDate { get; set; }

        public string? ImageUrl { get; set; }
        public string? imageThumbnailUrl { get; set; }
        

        public string Hall { get; set; }

        public bool IsAvailableForRental { get; set; }

        public string Description { get; set; }

        public IEnumerable<string> Categories { get; set; } = null; 
        public IEnumerable<BookCopyVM> BookCopies { get; set; } = null; 
        
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }


    }
}
