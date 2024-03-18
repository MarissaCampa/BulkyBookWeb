using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBook.Models.ViewModels
{
	public class UserVM
	{
		public ApplicationUser User { get; set; }
		[ValidateNever]
		public IEnumerable<SelectListItem> CompanyList { get; set; }
		public IEnumerable<SelectListItem> RoleList { get; set; }
	}
}
