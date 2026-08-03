namespace Bookify.Web.Core.Models
{
    public class Subscriptions
    {
        public int  Id { get; set; }
        public string? CreatedById { get; set; }
        public AppUser? CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);
        public int SubscriperId { get; set; }
        public Subscriper? Subscriper { get; set; }

    }

}
