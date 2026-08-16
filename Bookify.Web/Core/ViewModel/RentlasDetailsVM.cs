using Bookify.Web.Core.Models;
using Bookify.Web.Enums;

namespace Bookify.Web.Core.ViewModel
{
    public class RentlasDetailsVM
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }

        public IEnumerable<CopiesDetalisVm> Copies { get; set; } = new List<CopiesDetalisVm>();

    }
}
