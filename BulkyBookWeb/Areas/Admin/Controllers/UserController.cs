using BulkyBook.Models;
using BulkyBook.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using BulkyBook.Utility;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
			return View();
        }

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList= _db.ApplicationUsers.Include(u => u.Company).ToList();

			foreach (var user in objUserList)
			{
				if (user.Company == null)
				{
					user.Company = new() { Name = "" };
				}
			}

			return Json(new { data = objUserList });
		}

		public IActionResult Delete(int? id)
		{
			return Json(new { success = true, message = "Delete Successful" });
		}
		#endregion
	}
}
