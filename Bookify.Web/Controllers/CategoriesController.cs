using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        [AjaxFilter]
        public IActionResult Create()
        {
            return PartialView("_CreateAndEdit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAndEditCategoryVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(model);

            var category = new Category { Name = model.Name };
            _context.Add(category);
            _context.SaveChanges();
            return PartialView("_CategoryRow", category);
        }

        [HttpGet]
        [AjaxFilter]
        public IActionResult Edit(int id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
                return NotFound();

            var category = new CreateAndEditCategoryVM { Name = cat.Name, Id = id };
            return PartialView("_CreateAndEdit", category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateAndEditCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var cat = _context.Categories.FirstOrDefault(x => x.Id == model.Id);
            if (cat == null) return NotFound();
            cat.Name = model.Name;
            cat.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            TempData["Message"] = "Modified Successfully";
            return PartialView("_CategoryRow", cat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var cat = _context.Categories.FirstOrDefault(x => x.Id == id);
            if (cat == null)
                return NotFound();
            cat.IsDeleted = !cat.IsDeleted;
            cat.LastUpdatedOn = DateTime.Now;
            _context.SaveChanges();
            return Ok(cat.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(CreateAndEditCategoryVM model) 
        {
            var cat = _context.Categories.SingleOrDefault(c => c.Name == model.Name);
            var isAllowed = cat == null || model.Id == cat.Id; 
            return Json(isAllowed);

        }



    }

}

