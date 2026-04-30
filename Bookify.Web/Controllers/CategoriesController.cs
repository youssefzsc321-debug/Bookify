
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {
            
            var categories = _context.Categories.AsNoTracking().ToList();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("CreateAndEdit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAndEditCategoryVM model)
        {
            if (!ModelState.IsValid)
                return View("CreateAndEdit", model);

            var category = new Category { Name = model.Name };
            _context.Add(category);
            _context.SaveChanges();
            TempData["Message"] = "Save Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cat=_context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
                return NotFound();

            var category=new CreateAndEditCategoryVM { Name = cat.Name ,Id=id};
            return View("CreateAndEdit", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateAndEditCategoryVM model)
        {
            if(!ModelState.IsValid)
            {
                return View("CreateAndEdit", model);
            }
            var cat=_context.Categories.FirstOrDefault(x=>x.Id== model.Id);
            if (cat == null) return NotFound();
            cat.Name= model.Name;
            cat.LastUpdatedOn= DateTime.Now;    
            _context.SaveChanges();
            TempData["Message"] = "Modified Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var cat=_context.Categories.FirstOrDefault(x=>x.Id == id);
            if(cat == null)
                return NotFound();
            cat.IsDeleted=!cat.IsDeleted;
            cat.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(cat.LastUpdatedOn.ToString()); 
        }



    }

}

