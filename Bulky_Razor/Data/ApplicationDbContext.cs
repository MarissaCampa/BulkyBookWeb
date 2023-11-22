using Bulky_Razor.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky_Razor.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Category> Categories { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Category>().HasData(
					new Category { Id = 1, Name = "Fiction", DisplayOrder = 1 },
					new Category { Id = 2, Name = "Horror", DisplayOrder = 2 },
					new Category { Id = 3, Name = "Suspense", DisplayOrder = 3 }
				);
		}
	}
}
