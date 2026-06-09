using AutoMapper;
using Bookify.Web.Core.Models;

namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category
            CreateMap<Category, CategoryVM>();
            CreateMap<CreateAndEditCategoryVM, Category>().ReverseMap();

            //Author
            CreateMap<Authors, AuthorsVM>();
            CreateMap<CreateAndEditAuthorVM, Authors>().ReverseMap();
        }
    }
}