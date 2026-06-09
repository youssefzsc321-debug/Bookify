using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.Models
{

    [Index(nameof(Name),IsUnique = true)]
    public class Authors:BaseModel
    {
        public int Id { get; set; }
        public string  Name { get; set; }
       

    }
}
