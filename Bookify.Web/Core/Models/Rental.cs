namespace Bookify.Web.Core.Models
{
    public class Rental:BaseModel
    {
        public int Id { get; set; }

        public int SubscriperId { get; set; }
        public Subscriper? Subscriper { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Today;

        public bool PenaltyPay { get; set; }

        public ICollection<RentalCopies> RentalCopies { get; set; } = new List<RentalCopies>();


    }
}
