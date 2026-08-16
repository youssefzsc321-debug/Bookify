using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<Governrete> Governretes { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalCopies> RentalCopies { get; set; }
        public DbSet<Subscriper> Subscripers { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence<int>("SerialNumber", schema: "shared").StartsAt(100001).IncrementsBy(1); 
            builder.Entity<BookCopy>().Property(e => e.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");

            var cascadeFKS = builder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys())
                .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);
       
            foreach(var fk in cascadeFKS)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict ;
            }

            base.OnModelCreating(builder);
            builder.Entity<BookCategory>().HasKey(x => new { x.CategoryId, x.BookId });

            builder.Entity<Governrete>().ToTable("Governorates");
            builder.Entity<Area>().Property(e => e.GovernreteId).HasColumnName("GovernorateId");
            builder.Entity<RentalCopies>().HasKey(x => new { x.RentalId, x.BookCopyId });

            builder.Entity<Rental>().HasQueryFilter(r => !r.IsDeleted);
            builder.Entity<RentalCopies>().HasQueryFilter(r => !r.Rental!.IsDeleted);

          

        }

    }
}
