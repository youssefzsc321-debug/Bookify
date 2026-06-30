using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.ViewModel
{
    public class AuthorsVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime LastUpdatedOn { get; set; }
    }
}
