namespace Bookify.Web.Core.ViewModel
{
    public class RentalFormVM
    {
        public string SubscriberId { get; set; }

        public IList<int> SelectedCopies { get; set; } = new List<int>();

        public int? AllowedRentals { get; set; }
    }
}
