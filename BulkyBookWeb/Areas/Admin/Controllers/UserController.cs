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
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
			_userManager = userManager;
			_roleManager = roleManager;
        }

        public IActionResult Index()
        {
			return View();
        }

		public IActionResult RoleManagement(string userId)
		{
            RoleManagementVM RoleVM = new RoleManagementVM()
			{
				ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId, includeProperties: "Company"),
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
				{
					Text = i.Name,
					Value = i.Name
				}),
				CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
					Value = i.Id.ToString()
				})
			};

			RoleVM.ApplicationUser.Role = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == userId))
				.GetAwaiter().GetResult().FirstOrDefault();
			return View(RoleVM);
		}

		[HttpPost]
		public IActionResult RoleManagement(RoleManagementVM roleManagementVM)
		{
			//current user in the db
			string oldRole = _userManager.GetRolesAsync(_unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementVM.ApplicationUser.Id))
					.GetAwaiter().GetResult().FirstOrDefault();

            var applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == roleManagementVM.ApplicationUser.Id);

            if (roleManagementVM.ApplicationUser.Role != oldRole)
			{
                // a role was updated 
                if (roleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyId = null;
                }
				_unitOfWork.ApplicationUser.Update(applicationUser);
				_unitOfWork.Save();

				_userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
				_userManager.AddToRoleAsync(applicationUser, roleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();
			}
			else
			{
				if (oldRole == SD.Role_Company && applicationUser.CompanyId != roleManagementVM.ApplicationUser.CompanyId)
				{
					applicationUser.CompanyId = roleManagementVM.ApplicationUser.CompanyId;
					_unitOfWork.ApplicationUser.Update(applicationUser);
					_unitOfWork.Save();
				}
			}

			return RedirectToAction("Index");
		}

        #region API CALLS
        [HttpGet]
		public IActionResult GetAll()
		{
			List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

			foreach (var user in objUserList)
			{
				user.Role = _userManager.GetRolesAsync(user).GetAwaiter().GetResult().FirstOrDefault();

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
			var userFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
			if (userFromDb == null)
			{
				return Json(new { success = false, message = "Error while Locking/Unlocking" });
			}

			if (userFromDb.LockoutEnd != null && userFromDb.LockoutEnd > DateTime.Now)
			{
				// user is currently locked and we need to unlock them
				userFromDb.LockoutEnd = DateTime.Now;
			}
			else
			{
				userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
			}

			_unitOfWork.ApplicationUser.Update(userFromDb);
			_unitOfWork.Save();
			return Json(new { success = true, message = "Operation successful" });
		}

		#endregion
	}
}
