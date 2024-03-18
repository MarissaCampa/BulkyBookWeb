using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Models.ViewModels
{
	public class RoleManagementVM
	{
		public ApplicationUser ApplicationUser { get; set; }
		public IEnumerable<SelectListItem> RoleList { get; set; }
		public IEnumerable<SelectListItem> CompanyList { get; set; }
	}
}
