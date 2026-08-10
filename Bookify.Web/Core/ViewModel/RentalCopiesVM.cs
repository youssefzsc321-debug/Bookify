using Bookify.Web.Enums;

namespace Bookify.Web.Core.ViewModel
{
    public class RentalCopiesVM
    {
        public BookCopyVM BookCopy { get; set; }
        public DateTime RentalDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration);
        public DateTime? ReturnDate { get; set; }
        public DateTime? ExtendedOn { get; set; }

        public int DelayDays
        {

            get
            {
                var days = 0;
                if (ReturnDate.HasValue && ReturnDate.Value > EndDate)
                    days = (int)(ReturnDate.Value - EndDate).TotalDays;
                else if(!ReturnDate.HasValue && DateTime.Today > EndDate)
                    days = (int)(DateTime.Today - EndDate).TotalDays;
                return days;

            }

        }
    }
}
