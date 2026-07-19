using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Mapping;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookify.Web.Controllers
{

    [Authorize(Roles = AppRoles.Archive)]
    public class CategoriesController : Controller
    {
    
        private readonly IMapper _mapper;

        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public IActionResult Index()
        {
            var categories = _context.Categories.AsNoTracking().ToList();
            var categoriesVM = _mapper.Map<IEnumerable<CategoryVM>>(categories);
            return View(categoriesVM);
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

            var category = _mapper.Map<Category>(model);
            category.CreatedById= User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.Add(category);
            _context.SaveChanges();

            var catVM = _mapper.Map<CategoryVM>(category);
            return PartialView("_CategoryRow", catVM);
        }

        [HttpGet]
        [AjaxFilter]
        public IActionResult Edit(int id)
        {
            var cat = _context.Categories.FirstOrDefault(c => c.Id == id);
            if (cat == null)
                return NotFound();

            var category = _mapper.Map<CreateAndEditCategoryVM>(cat);
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
            var category = _context.Categories.FirstOrDefault(x => x.Id == model.Id);
            if (category == null) return NotFound();
            category = _mapper.Map(model, category);  
            category.LastUpdatedOn = DateTime.Now;
            category.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier).Value;
            _context.SaveChanges();
            var catVM = _mapper.Map<CategoryVM>(category);
            return PartialView("_CategoryRow", catVM);
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
            cat.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier).Value;
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

