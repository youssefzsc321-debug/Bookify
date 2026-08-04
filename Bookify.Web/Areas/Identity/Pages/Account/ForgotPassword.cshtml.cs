// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Models;
using Bookify.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Bookify.Web.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailBodyBulider emailBodyBulider;

        public ForgotPasswordModel(UserManager<AppUser> userManager, IEmailSender emailSender, IEmailBodyBulider emailBodyBulider)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                //var body = emailBodyBulider.GetBody("https://res.cloudinary.com/dhtvvjlko/image/upload/v1785135978/confirmed_hc5dxf.png",
                //   $"Hello {user.FullName}",
                //   "Please click the below button to reset your password",
                //   $"{HtmlEncoder.Default.Encode(callbackUrl)}",
                //   "Reset Password");

                var placeholders = new Dictionary<string, string>();
                placeholders.Add("[imageUrl]", "https://res.cloudinary.com/dhtvvjlko/image/upload/v1785135978/confirmed_hc5dxf.png");
                placeholders.Add("[header]", $"Hello {user.FullName}");
                placeholders.Add("[body]", $"Please click the below button to reset your password");
                placeholders.Add("[url]", $"{HtmlEncoder.Default.Encode(callbackUrl)}");
                placeholders.Add("[linkTitle]", "Reset Password");


                var body = emailBodyBulider.GetBody(EmailTempletes.Email, placeholders);


                

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    body);

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}
