using AutoMapper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class SearchController : Controller
    {
        
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IDataProtector _dataProtector;
        public SearchController(ApplicationDbContext context, IMapper mapper, IDataProtectionProvider dataProtector)
        {
           
            this.context = context;
            this.mapper = mapper;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(string key)
        {
            var bookId =int.Parse( _dataProtector.Unprotect(key));
            var book = context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .Include(b => b.Categories)
                .ThenInclude(b => b.Category)
                .FirstOrDefault(b => b.Id == bookId);

            if (book is null) return NotFound();


            var bookmodel = mapper.Map<BookDetailsVM>(book);


            bookmodel.AuthorsName = book.Author.Name;
            bookmodel.Categories = book.Categories.Select(c => c.Category.Name).ToList();

            return View(bookmodel);
            
        }

        public IActionResult Find(string query)
        {
            var books = context.Books
                .Include(b => b.Author)
                .Where(b => !b.IsDeleted && (b.Title.Contains(query) || b.Author.Name.Contains(query)))
                .Select(b => new
                {
                    Title = b.Title,
                    Author = b.Author.Name, 
                    Key = _dataProtector.Protect(b.Id.ToString())           
                })
                .ToList();

            return Ok(books);
        }
    }
}
