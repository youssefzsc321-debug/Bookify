namespace Bookify.Web.Core.ViewModel
{
    public class ReturnCopyVM
    {
        public int CopyId { get; set; }

        //[false : if don't want return and want extend , true: if want to return not extend]
        public bool? IsReturned { get; set; }
    }
}
