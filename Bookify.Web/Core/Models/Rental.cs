namespace Bookify.Web.Core.Models
{
    public class Rental
    {
        public int Id { get; set; }

        public int SubscriperId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Today;
        public Subscriper? Subscriper { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;
        public bool IsDeleted { get; set; }

        public DateTime? LastUpdatedOn { get; set; }


        public string? CreatedById { get; set; }
        public AppUser? CreatedBy { get; set; }

        public string? LastUpdatedById { get; set; }
        public AppUser? LastUpdatedBy { get; set; }

        public bool PenaltyPay { get; set; }

        public ICollection<RentalCopies> RentalCopies { get; set; } = new List<RentalCopies>();


    }
}
