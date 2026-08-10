using AutoMapper;
using Bookify.Web.Core.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper  mapper;
        public RentalsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(string id)
        {
            var modelVm = new RentalFormVM()
            {
                SubscriberId = id,
            };
                
            return View(modelVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxFilter]
        public IActionResult GetCopyDetails(SearchFormVM  model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var copy = _context.BookCopies.Include(b => b.Book).FirstOrDefault(b=>b.SerialNumber.ToString()==model.Value);
            if (copy is null) return NotFound(Errors.BookCopyNotFound);
            if (copy.IsDeleted || !copy.IsAvailableForRental || copy.Book.IsDeleted || !copy.Book.IsAvailableForRental) return BadRequest(Errors.BookCopyNotAvaliableForRental);

            //ToDo : check if this copy already in rental

            var modelVm = mapper.Map<BookCopyVM>(copy); 
            return PartialView("_copyDetails",modelVm);
        }



    }
}
