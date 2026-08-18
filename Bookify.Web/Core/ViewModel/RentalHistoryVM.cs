using Bookify.Web.Core.Models;
using Bookify.Web.Enums;

namespace Bookify.Web.Core.ViewModel
{
    public class RentalHistoryVM
    {
        public int BookCopyId { get; set; }
        public string? SubscriberName { get; set; }
        public string? SubscriberPhone { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? RentalDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool Extended { get; set; }
        public bool Delayed { get; set; }
        
    }
}
