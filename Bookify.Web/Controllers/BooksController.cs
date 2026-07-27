using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly IMapper mapper;
        private readonly IImageService imageService;
        private List<string> _allowedExtentions = new List<string>() { ".jpg", ".png", ".jepg" };
        private int _maxAllowSize = 2 * 1024 * 1024;


        private readonly Cloudinary _cloudinary;

        public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHost, IOptions<CloudinarySettings> cloudinary, IImageService imageService = null)
        {
            _context = context;
            this.mapper = mapper;
            _webHost = webHost;

            var account = new Account
            {
                Cloud = cloudinary.Value.CloudName,
                ApiKey = cloudinary.Value.APIKey,
                ApiSecret = cloudinary.Value.APISecret,
            };
            _cloudinary = new Cloudinary(account);
            this.imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetBooks()
        {


            IQueryable<Book> books = _context.Books.Include(b => b.Author).Include(b => b.Categories).ThenInclude(b => b.Category);

            var skip = int.Parse(Request.Form["start"]);

            var take = int.Parse(Request.Form["length"]);

            var sortColumnIndex = Request.Form["order[0][column]"];

            var sortColumnName = Request.Form[$"columns[{sortColumnIndex}][name]"];

            var sortColumnDir = Request.Form["order[0][dir]"];

            var searchValue = Request.Form["search[value]"];
            if (!string.IsNullOrEmpty(searchValue))
                books = books.Where(b => b.Title.Contains(searchValue) || b.Author.Name.Contains(searchValue));


            books = books.OrderBy(($"{sortColumnName} {sortColumnDir}"));


            var data = books.Skip(skip).Take(take).ToList();

            var mappedData = mapper.Map<IEnumerable<BookDetailsVM>>(data);
            var recordesTotal = books.Count();
            var recordsFiltered = recordesTotal;
            return Ok(new { recordesTotal = recordesTotal, recordsFiltered = recordsFiltered, data = mappedData });

        }
        public IActionResult Details(int id)
        {


            var book = _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCopies)
                .Include(b => b.Categories)
                .ThenInclude(b => b.Category)
                .FirstOrDefault(b => b.Id == id);

            if (book is null) return NotFound();


            var bookmodel = mapper.Map<BookDetailsVM>(book);


            bookmodel.AuthorsName = book.Author.Name;
            bookmodel.Categories = book.Categories.Select(c => c.Category.Name).ToList();

            return View(bookmodel);


        }
        [HttpGet]
        public IActionResult Create()
        {

            var viewmodel = FillModl();
            return View("Form", viewmodel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormVM model)
        {

            if (!ModelState.IsValid)
            {

                model = FillModl(model);
                return View("Form", model);
            }
            var book = mapper.Map<Book>(model);

            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            if (model.Image is not null)
            {
                
                var extension = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extension}";
                var folderName = "/Images/Books";
                var (isUploaded, erromessage) = await imageService.UploadAsync(model.Image, imageName, folderName, hasThumbnail: true);
                if (isUploaded)
                {
                    book.ImageUrl = $"/Images/Books/{imageName}";
                    book.imageThumbnailUrl = $"/Images/Books/Thumb/{imageName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Image), erromessage);
                    return View("From", FillModl(model));
                }




            }
            book.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            _context.Books.Add(book);
            _context.SaveChanges();


            return RedirectToAction(nameof(Details), new { id = book.Id });
        }


        private BookFormVM FillModl(BookFormVM? model = null)
        {
            BookFormVM Model = model is null ? new BookFormVM() : model;

            var authors = _context.Authors.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();
            var categories = _context.Categories.Where(x => !x.IsDeleted).OrderBy(x => x.Name).ToList();
            Model.Authors = mapper.Map<IEnumerable<SelectListItem>>(authors);
            Model.Categories = mapper.Map<IEnumerable<SelectListItem>>(categories);

            return Model;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(x => x.Id == id);
            if (book is null) return NotFound();
            var bookvm = mapper.Map<BookFormVM>(book);
            var model = FillModl(bookvm);
            model.SelectedCategories = book.Categories.Select(x => x.CategoryId).ToList();
            //model.AuthorsId= book.AuthorsId;

            return View("Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookFormVM model)
        {

            if (!ModelState.IsValid)
            {

                model = FillModl(model);
                return View("Form", model);
            }
            var book = _context.Books.Include(b => b.Categories).Include(b => b.BookCopies).SingleOrDefault(b => b.Id == model.Id);

            if (book == null) return NotFound();

            if (model.Image is not null)
            {

                if (book.ImageUrl is not null)
                {
                    imageService.Delete(book.ImageUrl, book.imageThumbnailUrl);
                }
                var extension = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extension}";
                var folderName = "/Images/Books";
                var (isUploaded, erromessage) = await imageService.UploadAsync(model.Image, imageName, folderName, hasThumbnail: true);
                if (isUploaded)
                {
                    model.ImageUrl = $"/Images/Books/{imageName}";
                    model.imageThumbnailUrl = $"/Images/Books/Thumb/{imageName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Image), erromessage);
                    return View("From", FillModl(model));
                }

            }
            else if (model.ImageUrl is null && book.ImageUrl is not null)
            {
                model.ImageUrl = book.ImageUrl;
                model.imageThumbnailUrl = book.imageThumbnailUrl;
            }
            book = mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;


            book.Categories.Clear();
            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });

            if (!book.IsAvailableForRental)
            {
                //var copies = book.BookCopies.Where(b => b.IsAvailableForRental == true).ToList();
                foreach (var c in book.BookCopies)
                {
                    c.IsAvailableForRental = false;
                }
            }

            _context.SaveChanges();


            return RedirectToAction(nameof(Details), new { id = book.Id });
        }

        public IActionResult AllowItem(BookFormVM model)
        {
            var book = _context.Books.SingleOrDefault(b => b.Title == model.Title && b.AuthorsId == model.AuthorsId);
            var isAllow = book is null || book.Id == model.Id;
            return Json(isAllow);
        }

        private string GetThumbnailUrl(string url)
        {
            var seperator = "/image/upload/";
            var parts = url.Split(seperator);

            var res = $"{parts[0]}{seperator}c_thumb,g_face,w_200,h_200/{parts[1]}";
            return res;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var book = _context.Books.FirstOrDefault(x => x.Id == id);
            if (book == null)
                return NotFound();
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.SaveChanges();
            return Ok(book.LastUpdatedOn.ToString());
        }
    }
}
