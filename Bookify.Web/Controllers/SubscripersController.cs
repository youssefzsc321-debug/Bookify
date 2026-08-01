using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IDataProtector _dataProtector;
        private readonly IMapper mapper;
        private readonly IImageService imageService;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtector)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageService = imageService;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchForSubscriper(SearchFormVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var sub = context.Subscripers.FirstOrDefault(s => s.MobileNumber == model.Value
                        || s.NationalId == model.Value
                        || s.Email == model.Value);

            if (sub == null)
                return PartialView("_DrawSubscriper", null);
            var modelVm = mapper.Map<SearchResultSusbscriperVM>(sub);

            modelVm.key=_dataProtector.Protect(sub.Id.ToString());

            return PartialView("_DrawSubscriper", modelVm);
        }

        public IActionResult Details(string id)
        {
            var subId = _dataProtector.Unprotect(id);
            var sub = context.Subscripers.Include(s => s.Area)
                .Include(s => s.Governrete)
               .FirstOrDefault(s => s.Id ==int.Parse(subId));
            if (sub is null) return NotFound();
            var modelVm = mapper.Map<SubscriberDetailsVM>(sub);
            modelVm.key = _dataProtector.Protect(sub.Id.ToString());
            ViewData["Id"] = subId;
            return View("Details", modelVm);

        }
        public IActionResult Create()
        {
            return View("Form", FillMolde());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubscriperFormVM model)
        {
            if (!ModelState.IsValid)
                return View("Form", FillMolde(model));

            var subscriper = mapper.Map<Subscriper>(model);
            subscriper.GovernreteId = model.SelectedGovernorate;
            subscriper.AreaId = model.SelectedArea;

            if (model.Image is not null)
            {
                //image,imageName,folder,hasThum
                var extention = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extention}";
                var folder = "/Images/Subscriper";
                var (IsUploaded, errorMesage) = await imageService.UploadAsync(model.Image, imageName, folder, true);
                if (IsUploaded)
                {
                    subscriper.ImageUrl = $"{folder}/{imageName}";
                    subscriper.imageThumbnailUrl = $"{folder}/Thumb/{imageName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Image), errorMesage);
                    return View("Form", FillMolde(model));

                }

            }
            subscriper.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            context.Subscripers.Add(subscriper);
            context.SaveChanges();
            var subId = _dataProtector.Protect(subscriper.Id.ToString());
            return RedirectToAction("Details", new { Id = subId });
        }

        public IActionResult Edit(string id)
        {
            var subId = int.Parse(_dataProtector.Unprotect(id));
            var subscriper = context.Subscripers.FirstOrDefault(s => s.Id == subId);
            if (subscriper == null) return NotFound();
            var modelVm = mapper.Map<SubscriperFormVM>(subscriper);

            modelVm.SelectedGovernorate = subscriper.GovernreteId;
            modelVm.SelectedArea = subscriper.AreaId;
            modelVm.Key = id;
           

            return View("Form", FillMolde(modelVm));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubscriperFormVM model)
        {
            if (!ModelState.IsValid)
                return View("Form", FillMolde(model));

            var suId = int.Parse(_dataProtector.Unprotect(model.Key));
            var subscriper = context.Subscripers.FirstOrDefault(s => s.Id == suId);
            if (subscriper is null) return NotFound();
            if (model.Image is not null)
            {
                if (subscriper.ImageUrl is not null)
                {
                    imageService.Delete(subscriper.ImageUrl, subscriper.imageThumbnailUrl);
                }
                var extention = Path.GetExtension(model.Image.FileName);
                var imageName = $"{Guid.NewGuid()}{extention}";
                var folder = "/Images/Subscriper";
                var (isUploaded, errorMessage) = await imageService.UploadAsync(model.Image, imageName, folder, true);
                if (isUploaded)
                {
                    model.ImageUrl = $"{folder}/{imageName}";
                    model.imageThumbnailUrl = $"{folder}/Thumb/{imageName}";
                }
                else
                {
                    ModelState.AddModelError(nameof(model.Image), errorMessage);
                    return View("Form", FillMolde(model));
                }
            }
            else
            {
                model.ImageUrl = subscriper.ImageUrl;
                model.imageThumbnailUrl = subscriper.imageThumbnailUrl;
            }


            subscriper = mapper.Map(model, subscriper);
            subscriper.AreaId = model.SelectedArea;
            subscriper.GovernreteId = model.SelectedGovernorate;
            subscriper.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            subscriper.LastUpdatedOn = DateTime.Now;
            context.Subscripers.Update(subscriper);
            context.SaveChanges();
            return RedirectToAction("Details", new { Id = model.Key });

        }


        private SubscriperFormVM FillMolde(SubscriperFormVM? modle = null)
        {
            var modelVM = (modle is null ? new SubscriperFormVM() : modle);
            modelVM.Governorates = context.Governretes.Where(g => !g.IsDeleted).OrderBy(g => g.Name)
                        .Select(g => new SelectListItem { Value = g.Id.ToString(), Text = g.Name });

            if (modelVM.SelectedGovernorate > 0)
            {
                modelVM.Areas = context.Areas.Where(a => !a.IsDeleted && a.GovernreteId == modelVM.SelectedGovernorate).OrderBy(a => a.Name)
                           .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();

            }
            else
                modelVM.Areas = new List<SelectListItem>();

            return modelVM;

        }

        [AjaxFilter]
        public IActionResult GetAreasAjax(int Governorateid)
        {
            var areas = context.Areas.Where(a => !a.IsDeleted && a.GovernreteId == Governorateid)
                .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Name }).ToList();
            return Ok(areas);
        }


        public IActionResult AllowEmail(SubscriperFormVM model)
        {
            var subId = 0;
            if(!string.IsNullOrEmpty(model.Key)) subId= int.Parse(_dataProtector.Unprotect(model.Key));
            var sub = context.Subscripers.FirstOrDefault(s => s.Email == model.Email);
            var valid = sub is null || subId == sub.Id;
            return Json(valid);
        }
        public IActionResult AllowMobile(SubscriperFormVM model)
        {
            var subId = 0;
            if (!string.IsNullOrEmpty(model.Key)) subId = int.Parse(_dataProtector.Unprotect(model.Key));
            var sub = context.Subscripers.FirstOrDefault(s => s.MobileNumber == model.MobileNumber);
            var valid = sub is null || subId == sub.Id;
            return Json(valid);
        }
        public IActionResult AllowNationalId(SubscriperFormVM model)
        {
            var subId = 0;
            if (!string.IsNullOrEmpty(model.Key)) subId = int.Parse(_dataProtector.Unprotect(model.Key));
            var sub = context.Subscripers.FirstOrDefault(s => s.NationalId == model.NationalId);
            var valid = sub is null || subId == sub.Id;
            return Json(valid);
        }




    }
}
