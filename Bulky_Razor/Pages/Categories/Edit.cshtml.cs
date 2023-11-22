using Bulky_Razor.Data;
using Bulky_Razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulky_Razor.Pages.Categories
{
	[BindProperties]
    public class EditModel : PageModel
    {
		private readonly ApplicationDbContext _db;
		public Category Category { get; set; }

		public EditModel(ApplicationDbContext db)
		{
			_db = db;
		}
		public void OnGet(int? id)
        {
			if (id != null & id > 0)
			{
				Category = _db.Categories.Find(id);
			}
		}
		public IActionResult OnPost()
		{
			if (Category == null)
			{
				return NotFound();
			}
			_db.Categories.Update(Category);
			_db.SaveChanges();
			TempData["success"] = "Category updated successfully";
			return RedirectToPage("Index");
		}
	}
}
