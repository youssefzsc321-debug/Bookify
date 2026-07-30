using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Core.ViewModel
{
    
    public class CreateAndEditCategoryVM
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = Errors.MaxLengthCharcters), Display(Name = "Category")]
        [Remote(action: "AllowItem", controller: "Categories", AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; }

    }
}
