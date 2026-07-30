using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.Models
{
    [Index(nameof(Name),IsUnique = true)]
    public class Governrete:BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }

        public ICollection<Area> Areas { get; set; } = new List<Area>(); 

        

    }
}
