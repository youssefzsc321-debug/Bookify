using Bookify.Web.BackgroungJobs;
using Bookify.Web.Core.Consts;
using Bookify.Web.Core.Mapping;
using Bookify.Web.Core.Models;
using Bookify.Web.Data;
using Bookify.Web.Helpers;
using Bookify.Web.Seeds;
using Bookify.Web.Services;
using Bookify.Web.Settings;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Threading.Tasks;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using WhatsAppCloudApi.Extensions;
using WhatsAppCloudApi.Services;

namespace Bookify.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));



            //builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.

                options.Password.RequireNonAlphanumeric = false;

                options.Password.RequiredLength = 8;
                options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+#";

            });

            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(8);
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.AllowedForNewUsers = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddDataProtection().SetApplicationName(nameof(Bookify));
            builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)));
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddExpressiveAnnotations();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaims>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IEmailBodyBulider, EmailBodyBulider>();
            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });
            builder.Services.AddWhatsAppApiClient(builder.Configuration);

            builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();
            builder.Services.AddScoped<HangfireTasks>();
            builder.Services.Configure<AuthorizationOptions>(options => options.AddPolicy("adminsOnly",
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole(AppRoles.Admin);
                    //policy.RequireUserName("");
                    //policy.RequireClaim("");

                }));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            var scopFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopFactory.CreateScope();
            var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManger = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            await DefaultRoles.SeedRoles(roleManger);
            await DefaultUsers.SeedAdminUser(userManger);

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                DashboardTitle = "Bookify Dashboard",
                IsReadOnlyFunc = (DashboardContext context) => true,

                Authorization = new IDashboardAuthorizationFilter[]
                {
                    new HangfireAuthorizationFilter("adminsOnly")
                }

            });
            //var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //var DataProtector = scope.ServiceProvider.GetRequiredService<IDataProtector>();
            //var EmailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
            //var EmailBodyBulider = scope.ServiceProvider.GetRequiredService<IEmailBodyBulider>();
            //var WhatsAppClient = scope.ServiceProvider.GetRequiredService<IWhatsAppClient>();
            //var WebHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            //var hangfireTasks = new HangfireTasks(dbcontext, DataProtector, EmailSender, EmailBodyBulider, WhatsAppClient, WebHostEnvironment);
            //RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 15 * * 1");

            var egyptTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");

            RecurringJob.AddOrUpdate<HangfireTasks>(
                recurringJobId: "send-expiration-alerts",
                methodCall: task => task.PrepareExpirationAlert(),
                cronExpression: Cron.Weekly(DayOfWeek.Wednesday, 16, 47), 
                options: new RecurringJobOptions
                {
                    TimeZone = egyptTimeZone
                }
            );


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
