using AutoMapper;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Categories
            CreateMap<Category, CategoryVM>();
            CreateMap<CreateAndEditCategoryVM, Category>().ReverseMap();
            CreateMap<Category, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            //Authors
            CreateMap<Authors, AuthorsVM>();
            CreateMap<CreateAndEditAuthorVM, Authors>().ReverseMap();
            CreateMap<Authors, SelectListItem>()
                .ForMember(dest=>dest.Value,opt=>opt.MapFrom(opt=>opt.Id))
                .ForMember(dest=>dest.Text,opt=>opt.MapFrom(opt=>opt.Name));

            //Books
            CreateMap<BookFormVM, Book>().ReverseMap()
                .ForMember(dest => dest.Categories, opt => opt.Ignore());


            CreateMap<Book, BookDetailsVM>()
                .ForMember(dest => dest.AuthorsName, opt => opt.MapFrom(opt => opt.Author.Name))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(opt => opt.Categories.Select(c => c.Category.Name).ToList()));

        }
    }
}