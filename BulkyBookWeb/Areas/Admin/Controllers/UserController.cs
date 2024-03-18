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
using BulkyBook.DataAccess.Repository;

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

		public IActionResult RoleManagement(string id)
		{
			UserVM userVM = new()
			{
				User = _db.ApplicationUsers.Include(u => u.Company).SingleOrDefault(u => u.Id == id),
				CompanyList = _db.Companies
				.GetAll().Select(u => new SelectListItem
				{
					Text = u.Name,
					Value = u.Id.ToString()
				}),
				Product = new Product()
			};

			return View();
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList= _db.ApplicationUsers.Include(u => u.Company).ToList();

			var userRoles = _db.UserRoles.ToList();
			var roles = _db.Roles.ToList();

			foreach (var user in objUserList)
			{
				var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
				user.Role = roles.FirstOrDefault(u => u.Id == roleId).Name;

				if (user.Company == null)
				{
					user.Company = new Company() { Name = "" };
				}
			}

			return Json(new { data = objUserList });
		}

		public IActionResult Delete(int? id)
		{
			return Json(new { success = true, message = "Delete Successful" });
		}

		[HttpPost]
		public IActionResult LockUnlock([FromBody]string id)
		{
			var objFromDb = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (objFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}

			if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
			{
				// user is currently locked and we need to unlock them
				objFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}
			_db.SaveChanges();
			return Json(new { success = true, message = "Operation successful" });
		}

		#endregion
	}
}
