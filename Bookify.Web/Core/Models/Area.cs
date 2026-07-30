using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name),nameof(GovernreteId),IsUnique =true)]
    public class Area:BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public int GovernreteId { get; set; }
        public Governrete? Governrete { get; set; }
      
    }
}
