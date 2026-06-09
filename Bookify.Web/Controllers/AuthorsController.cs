using AutoMapper;
using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;

        public AuthorsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var authores = _context.Authors.AsNoTracking().ToList();
            var authorsVm=mapper.Map<IEnumerable<AuthorsVM>>(authores);
            return View(authorsVm);
        }
        [HttpGet]
        [AjaxFilter]
        public IActionResult Create()
        {

            return PartialView("_AuthorForm");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateAndEditAuthorVM model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var author = mapper.Map<Authors>(model);
            _context.Authors.Add(author);
            _context.SaveChanges();
            var AuthVm = mapper.Map<AuthorsVM>(author);
            return PartialView("_AuthorRow", AuthVm);
        }

        [HttpGet]
        [AjaxFilter]
        public IActionResult Edit(int id)
        {
            var author = _context.Authors.FirstOrDefault(x=>x.Id == id);
            if (author == null) return NotFound();
            var authvm = mapper.Map<CreateAndEditAuthorVM>(author);
            return PartialView("_AuthorForm", authvm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateAndEditAuthorVM model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var auth=_context.Authors.FirstOrDefault(x=>x.Id==model.Id);
            if (auth == null) return NotFound();
            auth=mapper.Map(model,auth);
            auth.LastUpdatedOn= DateTime.Now;
            _context.SaveChanges();
            var authVm = mapper.Map<AuthorsVM>(auth);
            return PartialView("_AuthorRow", authVm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            var auth=_context.Authors.FirstOrDefault(x=>x.Id == id);
            if(auth == null) return NotFound();
            auth.LastUpdatedOn= DateTime.Now;
            auth.IsDeleted=!auth.IsDeleted;
            _context.SaveChanges();
            return Ok(auth.LastUpdatedOn.ToString());
        }

        public IActionResult AllowItem(CreateAndEditAuthorVM model)
        {
            var auth=_context.Authors.SingleOrDefault(a=>a.Name==model.Name);
            var allowed=auth==null||auth.Id==model.Id;
            return Json(allowed);
        }





    }
}
