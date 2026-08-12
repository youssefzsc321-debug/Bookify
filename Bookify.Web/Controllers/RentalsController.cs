using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Enums;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bookify.Web.Controllers
{
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly IDataProtector _dataProtector;
        public RentalsController(ApplicationDbContext context, IMapper mapper, IDataProtectionProvider dataProtector)
        {
            _context = context;
            this.mapper = mapper;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create(string subscriberKey)
        {
            var subid = int.Parse(_dataProtector.Unprotect(subscriberKey));
            var sub = _context.Subscripers.Include(s => s.Subscriptions)
                .Include(s => s.Rentals)
                .ThenInclude(s => s.RentalCopies).SingleOrDefault(s => s.Id == subid);
            //if(sub is null)return NotFound();
            //if (sub.BlacListed) return View("BlockedSubscriper",Errors.BlockedSubscriber);
            //if (sub.Subscriptions.LastOrDefault().EndDate < DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration))
            //    return View("SubscriptionWillExpire",Errors.SubscriptionWillExpire);

            //var currentRentals = sub.Rentals.SelectMany(s => s.RentalCopies).Count(s => !s.ReturnDate.HasValue);
            //var allowedRentals = (int)RentalConfigurations.MaxAllowedRetnals - currentRentals;

            //if (allowedRentals==0) return View("ReachedToMax", Errors.ReachedToMax);
            if (sub is null) return NotFound();
            var (title, error, allowedRentals) = validSubscriber(sub);
            if (!string.IsNullOrEmpty(error))
            {
                var modelError = new AlertVM()
                {
                    Title = title,
                    ErrorMessage = error,
                };
                return View("DeniedSubscription", modelError);
            }
            var modelVm = new RentalFormVM()
            {
                SubscriberId = subscriberKey,
                AllowedRentals = allowedRentals,
            };

            return View(modelVm);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentalFormVM model)
        {
            var subid = int.Parse(_dataProtector.Unprotect(model.SubscriberId));
            var sub = _context.Subscripers.Include(s => s.Subscriptions)
                .Include(s => s.Rentals)
                .ThenInclude(s => s.RentalCopies).SingleOrDefault(s => s.Id == subid);
            if (sub is null) return NotFound();
            var (title, error, allowedRentals) = validSubscriber(sub);
            if (!string.IsNullOrEmpty(error))
            {
                var modelError = new AlertVM()
                {
                    Title = title,
                    ErrorMessage = error,
                };
                return View("DeniedSubscription", modelError);
            }
            var copies = _context.BookCopies.Include(b => b.Book).Include(b => b.RentalCopies)
                .Where(c => model.SelectedCopies.Contains(c.Id)).ToList();

            var reantlasCopies = new List<RentalCopies>();
            foreach (var copy in copies)
            {
                var isExist = copy.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);
                if (isExist)
                {
                    var modelError = new AlertVM()
                    {
                        Title = "Already in rental!",
                        ErrorMessage = $"The{copy.Book.Title} book is already in another rentla!",
                    };
                    return View("DeniedSubscription", modelError);
                }
                if (!copy.IsAvailableForRental || copy.IsDeleted || copy.Book.IsDeleted || !copy.Book.IsAvailableForRental)
                {
                    var modelError = new AlertVM()
                    {
                        Title = "Book is deleted",
                        ErrorMessage = "This book is deleted!",
                    };
                    return View("DeniedSubscription", modelError);
                }
                var copyisalreadyExist = sub.Rentals.SelectMany(r => r.RentalCopies).Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);
                if (copyisalreadyExist)
                {
                    var modelError = new AlertVM()
                    {
                        Title = "Book is already exist",
                        ErrorMessage = $"The {copy.Book.Title} is already with you!",
                    };
                    return View("DeniedSubscription", modelError);

                }
                reantlasCopies.Add(new RentalCopies
                {
                    BookCopyId = copy.Id,
                    RentalDate = DateTime.Today
                });

                


            }
            sub.Rentals.Add(new Rental
            {
                RentalCopies = reantlasCopies,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value

            });
            _context.SaveChanges();
            return RedirectToAction("Details", "Subscripers", new { id = model.SubscriberId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxFilter]
        public IActionResult GetCopyDetails(SearchFormVM model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var copy = _context.BookCopies.Include(b => b.Book).FirstOrDefault(b => b.SerialNumber.ToString() == model.Value);
            if (copy is null) return NotFound(Errors.BookCopyNotFound);
            if (copy.IsDeleted || !copy.IsAvailableForRental || copy.Book.IsDeleted || !copy.Book.IsAvailableForRental) return BadRequest(Errors.BookCopyNotAvaliableForRental);

            //ToDo : check if this copy already in rental
            var isInAnotherRetnal = _context.RentalCopies.Any(b => b.BookCopyId == copy.Id && !b.ReturnDate.HasValue);
            if (isInAnotherRetnal) return BadRequest(Errors.InAnotherRetnal);
            var modelVm = mapper.Map<BookCopyVM>(copy);
            return PartialView("_copyDetails", modelVm);
        }
        private (string Title, string ErrorMessage, int? allowedRentals) validSubscriber(Subscriper sub)
        {


            if (sub.BlacListed) return ("Blocked Subscriber!", Errors.BlockedSubscriber, null);

            var lastSubscription = sub.Subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();

            if (lastSubscription is null || (lastSubscription.EndDate < DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration)))
                return ("Subscription Expiration!", Errors.SubscriptionWillExpire, null);


            var currentRentals = sub.Rentals.SelectMany(s => s.RentalCopies).Count(s => !s.ReturnDate.HasValue);
            var allowedRentals = (int)RentalConfigurations.MaxAllowedRetnals - currentRentals;

            if (allowedRentals == 0) return ("Reach to max allowed Rentals!", Errors.ReachedToMax, allowedRentals);
            return (null, null, allowedRentals);
        }


    }
}
