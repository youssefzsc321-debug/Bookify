using AutoMapper;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Core.ViewModel;
using Bookify.Web.Services;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Build.Logging.SimpleErrorLogger;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class UserController : Controller
    {
       
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IMapper mapper;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBulider emailBodyBulider;

        

        public UserController(UserManager<AppUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBulider emailBodyBulider)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            this.emailBodyBulider = emailBodyBulider;
        }
        public async Task<IActionResult> Index()
        {
            var users = await userManager.Users.ToListAsync();
            var model = mapper.Map<IEnumerable<UserVM>>(users);
           
            return View(model);
        }

        [AjaxFilter]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            
            var model = new UserFormVM()
            {
                Roles = await roleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToListAsync()
            };
          

            return PartialView("_Form", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormVM modelvm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var newUser = new AppUser()
            {
                FullName = modelvm.FullName,
                UserName = modelvm.UserName,
                Email = modelvm.Email,

                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value
                

            };

            var res = await userManager.CreateAsync(newUser, modelvm.Password); 
            if (res.Succeeded)  
            {
                foreach (var role in modelvm.SelectedRoles)
                {
                    await userManager.AddToRoleAsync(newUser, role);
                }

               
                var code = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = newUser.Id, code = code},
                    protocol: Request.Scheme);



                //var body = emailBodyBulider.GetBody("https://res.cloudinary.com/dhtvvjlko/image/upload/v1785055846/Hello-rafiki_vuxaue.png",
                //    $"Hello {newUser.FullName},Thanks for joining us",
                //    "Please Confirm Eamil", $"{HtmlEncoder.Default.Encode(callbackUrl)}",
                //    "Acvtive account");
                var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]","https://res.cloudinary.com/dhtvvjlko/image/upload/v1785055846/Hello-rafiki_vuxaue.png" },
                    {"[header]",$"Hello {newUser.FullName},Thanks for joining us" },
                    {"[body]", "Please Confirm Eamil"},
                    {"[url]",$"{HtmlEncoder.Default.Encode(callbackUrl)}" },
                    { "[linkTitle]","Acvtive account"}
                };
                var body = emailBodyBulider.GetBody(EmailTempletes.Email, placeholders);






                await _emailSender.SendEmailAsync(newUser.Email, "Confirm your email",body);

                var userViewModel = mapper.Map<UserVM>(newUser);
                return PartialView("_UserRow", userViewModel);

            }
            return BadRequest(string.Join(',', res.Errors.Select(e => e.Description)));


        }
        
        public async Task<IActionResult> Edit(string id)
        {
            var user=await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();

            var model = mapper.Map<UserFormVM>(user); 
            
            model.SelectedRoles = await userManager.GetRolesAsync(user);
            model.Roles = await roleManager.Roles.Select(r =>new SelectListItem() {Text=r.Name,Value=r.Name }).ToListAsync();
            return PartialView("_Form",model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserFormVM model)
        {

            var user = await userManager.FindByIdAsync(model.Id);
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(user is null) return NotFound();
            user = mapper.Map(model, user);
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            user.LastUpdatedOn = DateTime.Now;
            var res=await userManager.UpdateAsync(user);
            if(res.Succeeded)
            {
               
                var currnetRoles = await userManager.GetRolesAsync(user);
                var isSame = currnetRoles.SequenceEqual(model.SelectedRoles);
                if(!isSame)
                {
                    await userManager.RemoveFromRolesAsync(user,currnetRoles);
                    await userManager.AddToRolesAsync(user, model.SelectedRoles);
                }
                await userManager.UpdateSecurityStampAsync(user);
                var modelVm = mapper.Map<UserVM>(user);
                return PartialView("_UserRow", modelVm);


            }

            return BadRequest(string.Join(",", res.Errors.Select(e => e.Description)));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user =await userManager.FindByIdAsync(id);
            if (user is null) return NotFound();
            user.IsDeleted=!user.IsDeleted;
            user.LastUpdatedOn=DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await userManager.UpdateAsync(user);

            
            if (!result.Succeeded)
            {
                return BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));
            }
            if(user.IsDeleted)
            {
                await userManager.UpdateSecurityStampAsync(user);
            }
            return Ok(user.LastUpdatedOn.ToString());

        }



        [AjaxFilter]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user=await userManager.FindByIdAsync(id);
            if(user is null) return NotFound();
            var model=new ResetPasswordVM() {Id=id};
            return PartialView("_ResetPassword",model);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if(!ModelState.IsValid) return BadRequest();
            var user = await userManager.FindByIdAsync(model.Id);
            if(user is null) return NotFound();
            var currentPassword = user.PasswordHash;
            await userManager.RemovePasswordAsync(user); 
            var res =await userManager.AddPasswordAsync(user, model.NewPassword);
            if(res.Succeeded)
            {
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                user.LastUpdatedOn = DateTime.Now;
                await userManager.UpdateAsync(user);
                var modelVm = mapper.Map<UserVM>(user);
                return PartialView("_UserRow", modelVm);
            }
            
            user.PasswordHash=currentPassword;
            await userManager.UpdateAsync(user);
            return BadRequest(string.Join(',', res.Errors.Select(e => e.Description)));

            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnLock(string id)
        {
            var user=await userManager.FindByIdAsync(id); 
            if(user is null) return NotFound();
            await userManager.SetLockoutEndDateAsync(user, null);
            user.LastUpdatedOn= DateTime.Now;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest();

            return Ok(user.LastUpdatedOn.ToString());
        }

        public async Task<IActionResult> AllowUserName(UserFormVM model)
        {
            var user=await userManager.FindByNameAsync(model.UserName);
            var allow=user is null || model.Id==user.Id;
            return Json(allow);
        }
        public async Task<IActionResult> AllowUserEmail(UserFormVM model)
        {
            var user=await userManager.FindByEmailAsync(model.Email);
            var allow=user is null || model.Id==user.Id;
            return Json(allow);
        }
        

    }
}
