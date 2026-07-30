using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Core.ViewModel
{
    public class CreateAndEditAuthorVM
    {
        public int Id { get; set; }
        [MaxLength(100, ErrorMessage = Errors.MaxLengthCharcters), Display(Name = "Atuhor")]
        [Remote(action: "AllowItem", controller: "Authors", AdditionalFields = "Id", ErrorMessage = Errors.Duplicated)]
        public string Name { get; set; }

    }
}

