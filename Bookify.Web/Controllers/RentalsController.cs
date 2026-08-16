using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Enums;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
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

            return View("Form", modelVm);

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
            var reantlasCopies = new List<RentalCopies>();
            var copies = _context.BookCopies.Include(b => b.Book).Include(b => b.RentalCopies)
                .Where(c => model.SelectedCopies.Contains(c.SerialNumber)).ToList();

            foreach (var copy in copies)
            {
                 var (Title,ErrorMessage)=ValidateCopies(copy, model, sub);
                if(!string.IsNullOrEmpty(ErrorMessage))
                {
                    var modelError = new AlertVM()
                    {
                        Title= Title,
                        ErrorMessage = ErrorMessage,
                    };
                    return View("DeniedSubscription", modelError);
                }
                reantlasCopies.Add(new RentalCopies
                {
                    BookCopyId = copy.Id,
                    RentalDate = DateTime.Today
                });

            }


            var newRental = new Rental()
            {
                RentalCopies = reantlasCopies,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value
            };
            sub.Rentals.Add(newRental);
            _context.SaveChanges();
            return RedirectToAction("Details", new { id =newRental.Id });
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

        [HttpPost]
        [AjaxFilter]
        [ValidateAntiForgeryToken]
        public IActionResult MarkeDeleted(int id)
        {
            var rentla = _context.Rentals.Find(id);
            if (rentla is null || rentla.CreatedOn != DateTime.Today) return NotFound();
            rentla.IsDeleted = true;
            rentla.LastUpdatedOn = DateTime.Now;
            rentla.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.SaveChanges();
            return Ok();


        }

        public IActionResult Details(int id)
        {
            var rental = _context.Rentals.Include(r => r.RentalCopies)
                .ThenInclude(r => r.BookCopy)
                .ThenInclude(r => r.Book)
                .FirstOrDefault(r => r.Id == id);
            if (rental is null) return NotFound();
            var copies = new List<CopiesDetalisVm>();
            foreach (var ren in rental.RentalCopies)
            {
                var copy = new CopiesDetalisVm()
                {
                    RentalId = ren.RentalId,
                    BookCopyId = ren.BookCopy.Id,
                    BookId = ren.BookCopy.Book.Id,
                    BookTitle = ren.BookCopy.Book.Title,
                    EndDate = ren.EndDate,
                    ExtendedOn = ren.ExtendedOn,
                    PenaltyPay = rental.PenaltyPay,
                    RentalDate = ren.RentalDate,
                    ReturnDate = ren.ReturnDate,
                    StartDate = rental.StartDate,
                    imageThumbnailUrl = ren.BookCopy.Book.imageThumbnailUrl,
                    CreatedOn = rental.CreatedOn

                };

                copies.Add(copy);

            }
            var modelVm = new RentlasDetailsVM()
            {
                CreatedOn = rental.CreatedOn,
                Id = id,
                Copies = copies,
            };
            return View("Details", modelVm);


        }

        public IActionResult Edit(int id)
        {
            var rental = _context.Rentals
                .Include(r => r.RentalCopies)
                .ThenInclude(r => r.BookCopy).FirstOrDefault(r => r.Id == id);
            if (rental is null || rental.CreatedOn != DateTime.Today) return NotFound();
            var sub = _context.Subscripers.Include(s => s.Subscriptions)
                .Include(s => s.Rentals)
                .ThenInclude(s => s.RentalCopies)
                .SingleOrDefault(s => s.Id == rental.SubscriperId);

            if (sub is null) return NotFound();
            var (title, error, allowedRentals) = validSubscriber(sub, id);
            if (!string.IsNullOrEmpty(error))
            {
                var modelError = new AlertVM()
                {
                    Title = title,
                    ErrorMessage = error,
                };
                return View("DeniedSubscription", modelError);
            }
            var copies = rental.RentalCopies.Select(r => r.BookCopyId).ToList();
            var bookCopies = _context.BookCopies.Where(c => copies.Contains(c.Id)).Include(b => b.Book).ToList();


            var modelVm = new RentalFormVM()
            {
                Id = id,
                SubscriberId = _dataProtector.Protect(rental.SubscriperId.ToString()),
                AllowedRentals = allowedRentals,
                BookCopies = mapper.Map<IEnumerable<BookCopyVM>>(bookCopies)
            };
            return View("Form", modelVm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RentalFormVM model)
        {
            if (!ModelState.IsValid) return View("Form", model);
            var subid = int.Parse(_dataProtector.Unprotect(model.SubscriberId));
            var sub = _context.Subscripers.Include(s => s.Subscriptions)
                .Include(s => s.Rentals)
                .ThenInclude(s => s.RentalCopies).SingleOrDefault(s => s.Id == subid);
            if (sub is null) return NotFound();
            var (title, error, allowedRentals) = validSubscriber(sub, model.Id);
            if (!string.IsNullOrEmpty(error))
            {
                var modelError = new AlertVM()
                {
                    Title = title,
                    ErrorMessage = error,
                };
                return View("DeniedSubscription", modelError);
            }
            var currnetRental = _context.Rentals.Include(r => r.RentalCopies).FirstOrDefault(r => r.Id == model.Id);
            if (currnetRental is null) return NotFound();
            var copies = _context.BookCopies.Include(b => b.Book).Include(b => b.RentalCopies.Where(r => r.RentalId != model.Id))
                .Where(c => model.SelectedCopies.Contains(c.SerialNumber)).ToList();

            var reantlasCopies = new List<RentalCopies>();
            foreach (var copy in copies)
            {
                var (Title, ErrorMessage) = ValidateCopies(copy, model, sub,model.Id);
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    var modelError = new AlertVM()
                    {
                        Title = Title,
                        ErrorMessage = ErrorMessage,
                    };
                    return View("DeniedSubscription", modelError);
                }
                reantlasCopies.Add(new RentalCopies
                {
                    BookCopyId = copy.Id,
                    RentalDate = DateTime.Today
                });

            }
            currnetRental.RentalCopies.Clear();
            foreach (var copy in reantlasCopies)
            {
                currnetRental.RentalCopies.Add(copy);
            }
            currnetRental.LastUpdatedOn = DateTime.Today;
            currnetRental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.SaveChanges();
            return RedirectToAction("Details", new { id = model.Id });


        }
        private (string Title, string ErrorMessage, int? allowedRentals) validSubscriber(Subscriper sub, int? rentalId = null)
        {


            if (sub.BlacListed) return ("Blocked Subscriber!", Errors.BlockedSubscriber, null);

            var lastSubscription = sub.Subscriptions.OrderByDescending(s => s.EndDate).FirstOrDefault();

            if (lastSubscription is null || (lastSubscription.EndDate < DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration)))
                return ("Subscription Expiration!", Errors.SubscriptionWillExpire, null);


            var currentRentals = sub.Rentals.Where(r => rentalId is null || r.Id != rentalId)
                .SelectMany(s => s.RentalCopies).Count(s => !s.ReturnDate.HasValue);
            var allowedRentals = (int)RentalConfigurations.MaxAllowedRetnals - currentRentals;

            if (allowedRentals == 0) return ("Reach to max allowed Rentals!", Errors.ReachedToMax, allowedRentals);
            return (null, null, allowedRentals);
        }
        private (string Title, string ErrorMessage) ValidateCopies(BookCopy copy, RentalFormVM model, Subscriper sub, int?retnalId=null)
        {
            var isExist = copy.RentalCopies.Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue&&(retnalId ==null ||c.RentalId!= retnalId));
            if (isExist)
                return ("Already in rental!", $"The{copy.Book.Title} book is already in another rentla!");

            if (!copy.IsAvailableForRental || copy.IsDeleted || copy.Book.IsDeleted || !copy.Book.IsAvailableForRental)
                return ("Book is deleted", "This book is deleted!");

            var copyisalreadyExist = sub.Rentals.Where(r => (retnalId==null|| r.Id!= retnalId)).SelectMany(r => r.RentalCopies).Any(c => c.BookCopyId == copy.Id && !c.ReturnDate.HasValue);
            if (copyisalreadyExist)
                return ("Book is already exist", $"The {copy.Book.Title} is already with you!");

            return (null, null);

        }


    }
}
