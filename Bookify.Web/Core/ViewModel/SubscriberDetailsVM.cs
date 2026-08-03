using Bookify.Web.Core.Models;

namespace Bookify.Web.Core.ViewModel
{
    public class SubscriberDetailsVM
    {
        public string? key { get; set; }

        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public string? NationalId { get; set; }

        public string? MobileNumber { get; set; }

        public string? Email { get; set; }
        public string? imageThumbnailUrl { get; set; }

        public string? ImageUrl { get; set; }

        public string? Area { get; set; }

        public string? Governrete { get; set; }

        public string? Address { get; set; }

        public IEnumerable<SubscriptionsVM>? subscriptions { get; set; } = new List<SubscriptionsVM>();
        public bool BlacListed { get; set; }


    }
}
