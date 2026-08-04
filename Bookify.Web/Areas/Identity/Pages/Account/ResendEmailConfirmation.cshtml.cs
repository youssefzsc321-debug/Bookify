// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Bookify.Web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBulider emailBodyBulider;

        public ResendEmailConfirmationModel(UserManager<AppUser> userManager, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, IEmailBodyBulider emailBodyBulider)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _webHostEnvironment = webHostEnvironment;
            this.emailBodyBulider = emailBodyBulider;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            //[EmailAddress]
            public string UserName { get; set; }
        }

        public void OnGet(string userName)
        {
            Input=new InputModel() 
            {
                UserName = userName,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => (u.UserName == Input.UserName || u.Email == Input.UserName) && !u.IsDeleted);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);


            //var body = emailBodyBulider.GetBody("https://res.cloudinary.com/dhtvvjlko/image/upload/v1785055846/Hello-rafiki_vuxaue.png",
            //       $"Hello {user.FullName},Thanks for joining us",
            //       "Please Confirm Eamil", $"{HtmlEncoder.Default.Encode(callbackUrl)}",
            //       "Acvtive account");

            var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]","https://res.cloudinary.com/dhtvvjlko/image/upload/v1785055846/Hello-rafiki_vuxaue.png" },
                    {"[header]",$"Hello {user.FullName},Thanks for joining us" },
                    {"[body]", "Please Confirm Eamil"},
                    {"[url]",$"{HtmlEncoder.Default.Encode(callbackUrl)}" },
                    { "[linkTitle]","Acvtive account"}
                };
            var body = emailBodyBulider.GetBody(EmailTempletes.Email, placeholders);

            await _emailSender.SendEmailAsync(
               user.Email,
                "Confirm your email",
                body);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
