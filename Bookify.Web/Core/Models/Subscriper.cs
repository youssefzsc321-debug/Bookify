using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.Models
{

    [Index(nameof(NationalId),IsUnique = true)] 
    [Index(nameof(MobileNumber),IsUnique = true)]
    [Index(nameof(Email),IsUnique = true)]
    
    public class Subscriper:BaseModel
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [MaxLength(20)]
        public string NationalId { get; set; }
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        [MaxLength(150)]
        public string Email { get; set; }

        public bool HasWhatsApp { get; set; } 

        public string ImageUrl { get; set; }
        public string imageThumbnailUrl { get; set; }


        
        public int AreaId { get; set; }
        public Area? Area { get; set; }
        public int GovernreteId { get; set; }
        public Governrete? Governrete { get; set; }

        [MaxLength(500)]
        public string  Address { get; set; }

        public bool BlacListed { get; set; } 

    }
}
