using Bookify.Web.Enums;

namespace Bookify.Web.Core.ViewModel
{
    public class CopiesDetalisVm
    {
        public int RentalId { get; set; }
        public int BookCopyId { get; set; }
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;

        public DateTime RentalDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration);
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExtendedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? imageThumbnailUrl { get; set; }
        public bool PenaltyPay { get; set; }
        public int DelayDays
        {

            get
            {
                var days = 0;
                if (ReturnDate.HasValue && ReturnDate.Value > EndDate)
                    days = (int)(ReturnDate.Value - EndDate).TotalDays;
                else if (!ReturnDate.HasValue && DateTime.Today > EndDate)
                    days = (int)(DateTime.Today - EndDate).TotalDays;
                return days;

            }

        }

    }
}
