using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Bookify.Web.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        private readonly IMapper mapper;
        private List<string> _allowedExtentions = new List<string>() { ".jpg", ".png", ".jepg" };
        private int _maxAllowSize = 2 * 1024 * 1024;


        private readonly Cloudinary _cloudinary;

        public BooksController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHost, IOptions<CloudinarySettings> cloudinary)
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
        }

        public IActionResult Index()
        {
            return View();
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
                if (!_allowedExtentions.Contains(extension))
                {
                    model = FillModl(model);

                    ModelState.AddModelError(nameof(model.ImageUrl), errorMessage: Errors.NotAllowedExtention);
                    return View("Form", model);
                }
                if (model.Image.Length > _maxAllowSize)
                {
                    model = FillModl(model);
                    ModelState.AddModelError(nameof(model.Image), errorMessage: Errors.MaxSize);
                    return View("Form", model);
                }

                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHost.WebRootPath}/Images/Books", imageName);
                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
               
                stream.Dispose();

                book.ImageUrl = $"/Images/Books/{imageName}";
                book.imageThumbnailUrl = $"/Images/Books/Thumb/{imageName}";

                using var image = SixLabors.ImageSharp.Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = image.Height / ratio;
                image.Mutate(i => i.Resize(width: 200, height: (int)height)); 
                var Thumpath = Path.Combine($"{_webHost.WebRootPath}/Images/Books/Thumb", imageName); 
                image.Save(Thumpath);/ 
            }


            _context.Books.Add(book);
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
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
            if (book == null) return NotFound();
            var bookvm = mapper.Map<BookFormVM>(book);
            bookvm.SelectedCategories = book.Categories.Select(x => x.CategoryId).ToList();


            var model = FillModl(bookvm);
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
            var book = _context.Books.Include(b => b.Categories).SingleOrDefault(b => b.Id == model.Id);

            if (book == null) return NotFound();

            if (model.Image is not null)
            {

                if (book.ImageUrl is not null)
                {
                    var oldPathImage = $"{_webHost.WebRootPath}{book.ImageUrl}";
                    var oldPathThum = $"{_webHost.WebRootPath}{book.imageThumbnailUrl}";
                    if (System.IO.File.Exists(oldPathImage))
                    {
                        System.IO.File.Delete(oldPathImage);
                    }
                    if (System.IO.File.Exists(oldPathThum))
                    {
                        System.IO.File.Delete(oldPathThum);
                    }

                }
                var extension = Path.GetExtension(model.Image.FileName);
                if (!_allowedExtentions.Contains(extension))
                {
                    model = FillModl(model);

                    ModelState.AddModelError(nameof(model.ImageUrl), errorMessage: Errors.NotAllowedExtention);
                    return View("Form", model);
                }
                if (model.Image.Length > _maxAllowSize)
                {
                    model = FillModl(model);
                    ModelState.AddModelError(nameof(model.ImageUrl), errorMessage: Errors.MaxSize);
                    return View("Form", model);
                }

                var imageName = $"{Guid.NewGuid()}{extension}";
                var path = Path.Combine($"{_webHost.WebRootPath}/Images/Books", imageName);
                using var stream = System.IO.File.Create(path);
                await model.Image.CopyToAsync(stream);
              
                stream.Dispose();

           
                model.ImageUrl = $"/Images/Books/{imageName}";
                model.imageThumbnailUrl = $"/Images/Books/Thumb/{imageName}";

                using var image = SixLabors.ImageSharp.Image.Load(model.Image.OpenReadStream());
                var ratio = (float)image.Width / 200;
                var height = image.Height / ratio;
                image.Mutate(i => i.Resize(width: 200, height: (int)height));  
                var Thumpath = Path.Combine($"{_webHost.WebRootPath}/Images/Books/Thumb", imageName); 
                image.Save(Thumpath); 
            }
            else if (model.ImageUrl is null && book.ImageUrl is not null)
            {
                model.ImageUrl = book.ImageUrl;
            }
            book = mapper.Map(model, book);
            book.LastUpdatedOn = DateTime.Now;



            book.Categories.Clear();
            foreach (var category in model.SelectedCategories)
                book.Categories.Add(new BookCategory { CategoryId = category });
            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
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
    }
}
