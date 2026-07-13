using Bookify.Web.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bookify.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasSequence<int>("SerialNumber", schema: "shared").StartsAt(100001).IncrementsBy(1); 
            builder.Entity<BookCopy>().Property(e => e.SerialNumber).HasDefaultValueSql("NEXT VALUE FOR shared.SerialNumber");
            

            base.OnModelCreating(builder);
            builder.Entity<BookCategory>().HasKey(x => new { x.CategoryId, x.BookId });


        }

    }
}
