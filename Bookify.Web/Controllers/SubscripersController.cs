using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IDataProtector _dataProtector;
        private readonly IMapper mapper;
        private readonly IImageService imageService;
        private readonly IEmailSender emailSender;
        private readonly IEmailBodyBulider emailBodyBulider;
        private readonly IWhatsAppClient whatsAppClient;
        private readonly IWebHostEnvironment webHostEnvironment;

        public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtector, IEmailSender emailSender, IEmailBodyBulider emailBodyBulider, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.mapper = mapper;
            this.imageService = imageService;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            this.emailSender = emailSender;
            this.emailBodyBulider = emailBodyBulider;
            this.whatsAppClient = whatsAppClient;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
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

            modelVm.key = _dataProtector.Protect(sub.Id.ToString());

            return PartialView("_DrawSubscriper", modelVm);
        }

        public IActionResult Details(string id)
        {
            var subId = _dataProtector.Unprotect(id);
            var sub = context.Subscripers
                .Include(s => s.Area)
                .Include(s => s.Governrete)
                .Include(s => s.Subscriptions)
               .FirstOrDefault(s => s.Id == int.Parse(subId));
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
            var subscription = new Subscriptions()
            {
                CreatedById = subscriper.CreatedById,
                CreatedOn = subscriper.CreatedOn,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddYears(1),
            };
            subscriper.Subscriptions.Add(subscription);
            context.SaveChanges();
            //Send Email
            var placeholders = new Dictionary<string, string>() 
            {
                {"[logoUrl]","https://res.cloudinary.com/dhtvvjlko/image/upload/v1784985004/logo_xxv8zk.png" },
                {"[header]",$"Welcome aboard, {model.FirstName} {model.LastName}!" },
                {"[body]","We're thrilled to have you with us. Enjoy exploring everything Bookify has to offer!" }
            };
            var body = emailBodyBulider.GetBody(EmailTempletes.Notification, placeholders);
            await emailSender.SendEmailAsync(model.Email, "Welcome to Bookify! 👋", body);

            //Send Whatsapp message
            if (model.HasWhatsApp)
            {

                var components = new List<WhatsAppComponent>()
                {
                    new WhatsAppComponent
                    {
                        Type="header",
                        Parameters=new List<object>()
                        {
                            new WhatsAppTextParameter{Text="Youssef"}
                        }
                    }
                };
                var mobileNumber = (webHostEnvironment.IsDevelopment() ? "01202984092" : model.MobileNumber);

                var res = await whatsAppClient
                    .SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English_US, WhatsAppTempletes.WelcomeTemp, components);

            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewSubscription(string key)
        {
            var suId = int.Parse(_dataProtector.Unprotect(key));
            var sub = context.Subscripers.Include(s => s.Subscriptions).FirstOrDefault(s => s.Id == suId);
            if (sub is null) return NotFound();
            if (sub.BlacListed) return BadRequest();
            var lastSubscription = sub.Subscriptions.Last();
            var startDate = lastSubscription.EndDate < DateTime.Today ? DateTime.Today
                : lastSubscription.EndDate.AddDays(1);
            var newSubscrtiption = new Subscriptions()
            {
                StartDate = startDate,
                EndDate = startDate.AddYears(1),
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                CreatedOn = DateTime.Now,
            };

            sub.Subscriptions.Add(newSubscrtiption);
            context.SaveChanges();

            //Send Email

            var detailsUrl = Url.Action(
            action: "Details",
            controller: "Subscripers",
            values: new { id = key },
            protocol: Request.Scheme
            );

            var formattedEndDate = newSubscrtiption.EndDate.ToString("dd MMM yyyy");

            //var body = emailBodyBulider.GetBody(
            //    "https://res.cloudinary.com/dhtvvjlko/image/upload/v1785683951/undraw_online-party_uybk_lpgvot.png",
            //    $"Hello {sub.FirstName} {sub.LastName}!",
            //    $"Your subscription has been successfully renewed until {formattedEndDate}.",
            //    detailsUrl,
            //    "View Subscription Details"
            //);

            var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]","https://res.cloudinary.com/dhtvvjlko/image/upload/v1785683951/undraw_online-party_uybk_lpgvot.png" },
                    {"[header]",$"Hello {sub.FirstName} {sub.LastName}!" },
                    {"[body]", $"Your subscription has been successfully renewed until {formattedEndDate}."},
                    {"[url]",detailsUrl},
                    { "[linkTitle]", "View Subscription Details"}
                };
            var body = emailBodyBulider.GetBody(EmailTempletes.Email, placeholders);



            await emailSender.SendEmailAsync(
                sub.Email,
                "Subscription Renewed Successfully 🎉",
                body
            );


            var subVM = mapper.Map<SubscriptionsVM>(newSubscrtiption);
            return PartialView("_SubscritpionRow", subVM);

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
            if (!string.IsNullOrEmpty(model.Key)) subId = int.Parse(_dataProtector.Unprotect(model.Key));
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
