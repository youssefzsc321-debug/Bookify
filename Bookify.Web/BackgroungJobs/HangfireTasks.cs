using AutoMapper;
using Azure.Core;
using Bookify.Web.Core.Consts;
using Bookify.Web.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using WhatsAppCloudApi;
using WhatsAppCloudApi.Services;

namespace Bookify.Web.BackgroungJobs
{
    public class HangfireTasks
    {
        private readonly ApplicationDbContext context;
        private readonly IDataProtector _dataProtector;
        private readonly IEmailSender emailSender;
        private readonly IEmailBodyBulider emailBodyBulider;
        private readonly IWhatsAppClient whatsAppClient;
        private readonly IWebHostEnvironment webHostEnvironment;

        private readonly IConfiguration _configuration;
        private readonly LinkGenerator _linkGenerator;

        public HangfireTasks(ApplicationDbContext context, IDataProtectionProvider dataProtector, IEmailSender emailSender, IEmailBodyBulider emailBodyBulider, IWhatsAppClient whatsAppClient, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, LinkGenerator linkGenerator)
        {
            this.context = context;
            _dataProtector = dataProtector.CreateProtector("MySecureKey");
            this.emailSender = emailSender;
            this.emailBodyBulider = emailBodyBulider;
            this.whatsAppClient = whatsAppClient;
            this.webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _linkGenerator = linkGenerator;
        }

        public async Task PrepareExpirationAlert()
        {
            var subs = context.Subscripers.Include(s => s.Subscriptions)
                .Where(s => !s.BlacListed && s.Subscriptions.OrderByDescending(s => s.EndDate)
                .FirstOrDefault().EndDate.AddDays(-5) == DateTime.Today).ToList();


            var baseUrl = _configuration["AppConfig:BaseUrl"];

            foreach (var sub in subs)
            {
                var path = _linkGenerator.GetPathByAction(
                    action: "Details",
                    controller: "Subscripers",
                    values: new { id = _dataProtector.Protect(sub.Id.ToString()) }
                );
                var detailsUrl = $"{baseUrl}{path}";
                var placeholders = new Dictionary<string, string>()
{
                         { "[imageUrl]", "https://res.cloudinary.com/dhtvvjlko/image/upload/v1785921296/Urgent-pana_m8wctg.png" },

                         { "[header]", $"Hello {sub.FirstName} {sub.LastName}!" },

                         { "[body]", sub.Subscriptions.LastOrDefault().EndDate.ToString("dd MMM yyyy") },

                         { "[url]", detailsUrl },

                         { "[linkTitle]", "Renew Subscription Now" }
                };
                var body = emailBodyBulider.GetBody(EmailTempletes.ExpirationEmail, placeholders);
                await emailSender.SendEmailAsync(sub.Email, "⏳ Your Subscription is Expiring Soon!", body);


                //Send Whatsapp message
                if (sub.HasWhatsApp)
                {

                    var components = new List<WhatsAppComponent>()
                {
                    new WhatsAppComponent
                    {
                        Type="header",
                        Parameters=new List<object>()
                        {
                            new WhatsAppTextParameter{Text=sub.FirstName}
                        }
                    }
                };
                    var mobileNumber = (webHostEnvironment.IsDevelopment() ? "01202984092" : sub.MobileNumber);


                    await whatsAppClient
                        .SendMessage($"2{mobileNumber}", WhatsAppLanguageCode.English, WhatsAppTempletes.WelcomeTemp, components);

                }
            }
        }

        public async Task PrepareExpirationRentals()
        {
            var tomorrow = DateTime.Today.AddDays(1);
            var rentals = context.Rentals
                .Include(r => r.Subscriper)
                .Include(r => r.RentalCopies)
                .ThenInclude(c => c.BookCopy)
                .ThenInclude(c => c.Book)
                .Where(r=>!r.IsDeleted&&r.RentalCopies.Any(c=>!c.ReturnDate.HasValue&&c.EndDate.Date==tomorrow)).ToList();
            var baseUrl = _configuration["AppConfig:BaseUrl"];

            foreach (var ren in rentals)
            {
                
                if (ren is null) continue;

                var path = _linkGenerator.GetPathByAction(
                    action: "Details",
                    controller: "Rentals",
                    values: new { id =ren.Id }
                );

                var detailsUrl = $"{baseUrl}{path}";

                var placeholders = new Dictionary<string, string>()
                {
                    { "[imageUrl]", "https://res.cloudinary.com/dhtvvjlko/image/upload/v1785921296/Urgent-pana_m8wctg.png" },
                    { "[header]", $"Hello {ren.Subscriper.FirstName} {ren.Subscriper.LastName}!" },
                    { "[body]", $"Your rental return date is tomorrow ({tomorrow:dd MMM yyyy}). Please return or extend your copies on time." },
                    { "[url]", detailsUrl },
                    { "[linkTitle]", "View Rental Details" }
                };

                var body = emailBodyBulider.GetBody(EmailTempletes.ExpirationEmail, placeholders);
                await emailSender.SendEmailAsync(ren.Subscriper.Email, "⏳ Your Rental is Expiring Tomorrow!", body);
            }

        }
    }
}
