using AutoMapper;
using Bookify.Web.Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bookify.Web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IDataProtector _dataProtector;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IMapper mapper, IDataProtectionProvider dataProtector)
        {
            _logger = logger;
            this.context = context;
            this.mapper = mapper;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
        }


        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction("Index", "Dashboard");
            var lastAddedBooks = mapper.Map<IEnumerable<BookDetailsVM>>(context.Books
                 .Include(b => b.Author)
                 .Where(b => !b.IsDeleted)
                 .OrderByDescending(b => b.Id)
                 .Take(8)
                 .ToList());

            foreach (var book in lastAddedBooks)
                book.key = _dataProtector.Protect(book.Id.ToString());

            return View(lastAddedBooks);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
