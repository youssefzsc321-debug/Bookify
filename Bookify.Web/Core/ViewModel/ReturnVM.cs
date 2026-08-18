using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace Bookify.Web.Core.ViewModel
{
    public class ReturnVM
    {
        public int RentalId { get; set; }
        public IList<RentalCopiesVM> copies { get; set; } = new List<RentalCopiesVM>();
        public List<ReturnCopyVM> SelectedCopies { get; set; }

        public bool AllowExtend { get; set; }
        [AssertThat("(TotalDelayDays==0&&PenaltyPaid==false)||(PenaltyPaid==true)", ErrorMessage = "\"You must confirm collecting the late penalty fees.\"")]
        public bool PenaltyPaid { get; set; }
        public int TotalDelayDays
        {
            get => copies.Sum(s => s.DelayDays);
        }
    }
}
