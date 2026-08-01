using AutoMapper;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity;
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

            CreateMap<BookCopy, BookCopyVM>()
                .ForMember(dest => dest.BookTilte, opt => opt.MapFrom(opt => opt.Book.Title));

            CreateMap<BookCopy, BookCopyForm>();

            CreateMap<AppUser, UserVM>()
                .ForMember(dest=>dest.IsLocked,opt=>opt.MapFrom(src=>src.LockoutEnd.HasValue&&src.LockoutEnd > DateTimeOffset.UtcNow));


            CreateMap<UserFormVM, AppUser>()
                .ForMember(dest=>dest.NormalizedEmail,opt=>opt.MapFrom(src=>src.Email.ToUpper()))
                .ForMember(dest=>dest.NormalizedUserName,opt=>opt.MapFrom(src=>src.UserName.ToUpper()))
                .ReverseMap();

            CreateMap<Governrete, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(opt => opt.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(opt => opt.Name));

            CreateMap<Area, SelectListItem>()
                .ForMember(dest => dest.Value, opt => opt.MapFrom(opt => opt.Id))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(opt => opt.Name));


            CreateMap<SubscriperFormVM, Subscriper>()
                .ReverseMap();
                

            CreateMap<Subscriper, SubscriberDetailsVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Area, opt => opt.MapFrom(src => src.Area.Name))
                .ForMember(dest => dest.Governrete, opt => opt.MapFrom(src => src.Governrete.Name));
            CreateMap<Subscriper, SearchResultSusbscriperVM>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
            






        }
    }
}