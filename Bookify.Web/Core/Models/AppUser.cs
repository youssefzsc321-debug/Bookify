using Microsoft.AspNetCore.Identity;

namespace Bookify.Web.Core.Models
{
    public class AppUser:IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime? LastUpdatedOn { get; set; }

        public string? CreatedById { get; set; }
        public string? LastUpdatedById { get; set; }
    }
}
