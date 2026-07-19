namespace Bookify.Web.Core.ViewModel
{
    public class UserVM
    {
        public string Id { get; set; } 
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }
}
