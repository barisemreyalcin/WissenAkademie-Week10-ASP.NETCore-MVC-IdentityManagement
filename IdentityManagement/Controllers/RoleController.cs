using System.ComponentModel.DataAnnotations;
using IdentityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.Controllers
{
	public class RoleController : Controller
	{
		// User yönetmek için: UserManager
		// Login Logout yönetmek için: SignInManager
		// User yönetmek için: RoleManager
		private RoleManager<IdentityRole> roleManager;
		private UserManager<AppUser> userManager { get; set;}

		public RoleController(RoleManager<IdentityRole> _roleManager, UserManager<AppUser> _userManager)
		{
			this.roleManager = _roleManager;
			this.userManager = _userManager;
		}

		public IActionResult Index()
		{
			// Burda rolleri listeleyecem
			return View(roleManager.Roles);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create([Required] string name) // input name ile aynı isim
		{
			if (ModelState.IsValid)
			{
				IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Role");
				}
				else
					Errors(result);
			}
			return View((object)name);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			IdentityRole role = await roleManager.FindByIdAsync(id);
			if (role != null)
			{
				IdentityResult result = await roleManager.DeleteAsync(role);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Role");
				}
				else Errors(result);
			}
			else
				ModelState.AddModelError("", "Role Not Found");

			return View("Index", roleManager.Roles);
		}

		[HttpPost]
		public async Task<IActionResult> Update(RoleModification model)
		{
			IdentityResult result;
			if(ModelState.IsValid)
			{
				// role ekleme
				foreach (string userId in model.AddIds ?? new string[] {})
				{
					// checkbox ile seçtiklerim buraya array olarak gelir
					AppUser user = await userManager.FindByIdAsync(userId);
					if (user != null)
					{
						result = await userManager.AddToRoleAsync(user, model.RoleName);

						if(!result.Succeeded)
							Errors(result);	
					}
				}

				// rolden silme
				foreach (string userId in model.DeleteIds ?? new string[] {})
				{
					AppUser user = await userManager.FindByIdAsync(userId);
					if (user != null)
					{
						result = await userManager.RemoveFromRoleAsync(user, model.RoleName);

						if(!result.Succeeded)
							Errors(result);	
					}
				}
			}

			if (ModelState.IsValid)
				return RedirectToAction(nameof(Index), "Role");
			else
				return await Update(model.RoleId); // get olan update method

		}

		public async Task<IActionResult> Update(string id)
		{
			IdentityRole role = await roleManager.FindByIdAsync(id);
			List<AppUser> members = new List<AppUser>();
			List<AppUser> nonMembers = new List<AppUser>();

			foreach (var user in userManager.Users)
			{
				var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
				list.Add(user);
			}

			return View(new RoleEdit
			{
				Role = role,
				Members = members,
				NonMembers = nonMembers
			});
		}

		private void Errors(IdentityResult result)
		{
			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError("", $"{error.Code}: {error.Description}");
			}
		}
	}
}
