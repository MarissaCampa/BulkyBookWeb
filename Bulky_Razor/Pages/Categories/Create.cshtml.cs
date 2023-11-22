using Bulky_Razor.Data;
using Bulky_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky_Razor.Pages.Categories
{
	[BindProperties]
    public class CreateModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		public Category Category { get; set; }

		public CreateModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet()
        {
        }
		public IActionResult OnPost()
		{
			_db.Add(Category);
			_db.SaveChanges();
			TempData["success"] = "Category created successfully";
			return RedirectToPage("Index");
		}
	}
}
