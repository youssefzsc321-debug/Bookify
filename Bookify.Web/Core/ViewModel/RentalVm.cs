using Bookify.Web.Core.Models;

namespace Bookify.Web.Core.ViewModel
{
    public class RentalVm
    {
        public int Id { get; set; }
        public int SubscriberId { get; set; }
        public string? SubscriberFullName { get; set; }
        public DateTime StartDate { get; set; }
        public IEnumerable<RentalCopiesVM>? RentalCopies { get; set; } = new List<RentalCopiesVM>();
        public DateTime CreatedOn { get; set; } = DateTime.Today;
        public int CountOfCopies { get => RentalCopies?.Count() ?? 0; }
        public int TotalDelayDays { get => RentalCopies?.Sum(c => c.DelayDays) ?? 0; }
        public int TotalCopies { get => RentalCopies.Count(); }
        public bool IsDeleted { get; set; }

        public DateTime? LastUpdatedOn { get; set; }

    }




}
