using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.ViewModel
{
    
    public class CreateAndEditCategoryVM
    {
        public int Id { get; set; }
        [MaxLength(100,ErrorMessage ="Max Length Is 100 Charcter"),MinLength(3,ErrorMessage ="Enter at least 3 charcters")]

        [Remote(action: "AllowItem", controller: "Categories", AdditionalFields = "Id", ErrorMessage = "This Category is exist")]

        public string Name { get; set; } = null!;
    }
}
