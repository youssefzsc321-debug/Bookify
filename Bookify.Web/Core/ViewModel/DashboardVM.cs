namespace Bookify.Web.Core.ViewModel
{
    public class DashboardVM
    {
        public int NumberOfBooks { get; set; }
        public int NumberOfSubscribers { get; set; }

        public IEnumerable<BookDetailsVM> LastAddedBooks { get; set; }=new List<BookDetailsVM>();
        public IEnumerable<BookDetailsVM> TopBooks { get; set; }= new List<BookDetailsVM>();
    }
}
