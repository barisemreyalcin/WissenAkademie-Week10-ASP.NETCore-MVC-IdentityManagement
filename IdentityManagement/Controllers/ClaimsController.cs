using System.Security.Claims;
using IdentityManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.Controllers
{
	[Authorize]
	public class ClaimsController : Controller
	{
		private UserManager<AppUser> userManager;

		public ClaimsController(UserManager<AppUser> _userManager)
		{
			this.userManager = _userManager;
		}

		public IActionResult Index()
		{
			//Bu view'a users'ın claim'leri gönderelim. User login olduktan sonra httpContext'ten bilgiler alınabilir
			return View(User?.Claims);
		}

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[ActionName("Create")]
		public async Task<IActionResult> Create_Post(string claimType, string claimValue)
		{
			AppUser user = await  userManager.GetUserAsync(HttpContext.User);
			Claim claim = new Claim(claimType, claimValue, ClaimValueTypes.String);
			IdentityResult result = await userManager.AddClaimAsync(user, claim);

			if (result.Succeeded)
				return RedirectToAction("Index", "Claims");
			else
				Errors(result);

			return View();
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
