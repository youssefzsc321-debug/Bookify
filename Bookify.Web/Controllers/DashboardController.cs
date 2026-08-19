using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Bookify.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        public DashboardController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var topBooks = context.RentalCopies
                .Include(r => r.BookCopy)
                .ThenInclude(r => r.Book)
                .ThenInclude(r => r.Author)
                .Where(r=>!r.BookCopy.Book.IsDeleted)
                .GroupBy(b => new {BookId= b.BookCopy.BookId,BookTitle= b.BookCopy.Book.Title, BookImage=b.BookCopy.Book.imageThumbnailUrl, AuthorName = b.BookCopy.Book.Author.Name })
                .Select(g => new
                {
                    Title=g.Key.BookTitle,
                    Id=g.Key.BookId,
                    AuthorsName=g.Key.AuthorName,
                    imageThumbnailUrl=g.Key.BookImage,
                    count=g.Count()
                }).OrderByDescending(b=>b.count)
                .Take(6)
                .Select(b=>new BookDetailsVM 
                {
                    Id=b.Id,
                    AuthorsName=b.AuthorsName,
                    imageThumbnailUrl=b.imageThumbnailUrl,
                    Title=b.Title,
                })
                .ToList();
            var modelVm = new DashboardVM 
            {
                NumberOfBooks=context.Books.Count(c=>!c.IsDeleted),
                NumberOfSubscribers=context.Subscripers.Count(c=>!c.IsDeleted),
                LastAddedBooks=mapper.Map<IEnumerable<BookDetailsVM>>(context.Books
                .Include(b=>b.Author)
                .Where(b=>!b.IsDeleted)
                .OrderByDescending(b=>b.Id)
                .Take(8)
                .ToList()),
                TopBooks= topBooks
            };



            return View(modelVm);
        }

        [AjaxFilter]
        public IActionResult GetRentalPerDay(DateTime? StartDate, DateTime? EndDate )
        {
           var startDate=StartDate??DateTime.Today.AddDays(-29);
           var endDate=EndDate??DateTime.Today;
            var rentlasPerDays = context.RentalCopies
                .Where(r => r.RentalDate >= startDate && r.RentalDate <= endDate)
                .GroupBy(r =>  r.RentalDate.Date)
                .Select(g => new 
                {
                    Date = g.Key,
                    Value = g.Count()
                }).ToDictionary(x=>x.Date, x=>x.Value);

            var charDate = new List<ChartItemVM>();
            for (var day= startDate; day<= endDate; day=day.AddDays(1))
            {
                charDate.Add(new ChartItemVM
                {
                    Label = day.ToString("d MMM"),
                    Value = rentlasPerDays.TryGetValue(day.Date, out var res) ? res : 0
                });

            }


            return Ok(charDate);
        }

        public IActionResult GetSubscribersPerCity()
        {
            var subscribersPerCites = context.Subscripers
                .Where(s=>!s.IsDeleted)
                .Include(s => s.Governrete)
                .GroupBy(s => new { s.GovernreteId,Name=s.Governrete.Name })
                .Select(g => new
                {
                    CityName=g.Key.Name,
                    count=g.Count()
                }).ToList();
            return Ok(subscribersPerCites);
        }

    }
}
