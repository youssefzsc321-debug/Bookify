using AutoMapper;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Numerics;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookify.Web.Controllers
{
    public class BookCopiesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BookCopiesController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        public IActionResult Index()
        {
            return View();
        }

        [AjaxFilter]
        public IActionResult Create(int bookId)  
        {
            var originBook = context.Books.FirstOrDefault(b => b.Id == bookId);
            if (originBook is null) return NotFound();
            var viewmodel = new BookCopyForm() { BookId = bookId, ShowRentalInput = originBook.IsAvailableForRental };
            return PartialView("Form", viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopyForm model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var originBook = context.Books.Find(model.BookId);
            if (originBook is null) return NotFound();

            var copy = new BookCopy()
            {
                Id = model.Id,
                EditionNumber = model.EditionNumber,
                IsAvailableForRental = model.IsAvailableForRental,
                BookId = model.BookId,
            };

            context.BookCopies.Add(copy);
            context.SaveChanges();
          
            var modelVm = mapper.Map<BookCopyVM>(copy);
            return PartialView("_BookCopyRow", modelVm);
            
        }
        [AjaxFilter]
        public IActionResult Edit(int id)
        {
            var copy = context.BookCopies.Find(id);
            if (copy is null) return NotFound();
            var originBook = context.Books.Find(copy.BookId);
            if(originBook is null)return NotFound();
            var model=mapper.Map<BookCopyForm>(copy);
            model.ShowRentalInput = originBook.IsAvailableForRental;
            return PartialView("Form",model);
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookCopyForm model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var copy = context.BookCopies.Find(model.Id);
            if(copy is null) return NotFound();
            var originBook = context.Books.Find(model.BookId);
            if(originBook is null) return NotFound();
            copy.EditionNumber=model.EditionNumber;
            copy.IsAvailableForRental=model.IsAvailableForRental;
            copy.LastUpdatedOn=DateTime.Now;
            context.SaveChanges();
            var modelVm=mapper.Map<BookCopyVM>(copy);
            return PartialView("_BookCopyRow", modelVm);
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var copy = context.BookCopies.FirstOrDefault(x => x.Id == id);
            if (copy is null) return NotFound();
            copy.IsDeleted = !copy.IsDeleted;
            copy.LastUpdatedOn = DateTime.Now;
            context.SaveChanges();
            return Ok(copy.LastUpdatedOn.ToString());

        }

    }
}
