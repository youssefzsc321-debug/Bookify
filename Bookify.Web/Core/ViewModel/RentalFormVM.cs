namespace Bookify.Web.Core.ViewModel
{
    public class RentalFormVM
    {
        public int Id { get; set; }
        public string SubscriberId { get; set; }

        public IList<int> SelectedCopies { get; set; } = new List<int>();

        public IEnumerable<BookCopyVM>?BookCopies { get; set; }=new List<BookCopyVM>();
        public int? AllowedRentals { get; set; }
    }
}
