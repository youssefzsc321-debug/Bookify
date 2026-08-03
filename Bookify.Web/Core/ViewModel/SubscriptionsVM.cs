namespace Bookify.Web.Core.ViewModel
{
    public class SubscriptionsVM
    {
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);

        public string Status 
        {
            get
            {
                return DateTime.Today > EndDate ? "Expired" :
                    DateTime.Today < StartDate ? "Not Started" : "Active";
            }
        }
    }
}
