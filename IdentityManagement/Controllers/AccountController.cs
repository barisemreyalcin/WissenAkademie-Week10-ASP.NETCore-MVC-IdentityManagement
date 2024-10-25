using IdentityManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.Controllers
{
	[Authorize] // Herhangi bir action çalışması için bir login işlemi gerekir.
	public class AccountController : Controller
	{
		private UserManager<AppUser> userManager;
		private SignInManager<AppUser> signInManager;

		// Yukardakileri controller'ın constructor'ına inject edecem
		public AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager)
		{
			this.userManager = _userManager;
			this.signInManager = _signInManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[AllowAnonymous] // Authorize etkisi kalksın ve herkese erişilebilir olsun diye
		// Buraya ulaşmak için bana login olmam için görünecek olan view'ı sağlayan action
		public IActionResult Login(string returnUrl)
		{
			Login login = new Login();
			login.ReturnUrl = returnUrl;
			return View(login);
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(Login login)
		{
			if(ModelState.IsValid)
			{
				AppUser appUser = await userManager.FindByEmailAsync(login.Email);

				if(appUser != null)
				{
					// Login process'i başlatıyorum

					// burada aynı user farklı makinelerde login olmuşsa sadece biri açık olsun diye
					await signInManager.SignOutAsync();

					Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, login.RememberMe, false); // sondakiler beni hatırla ve çoklu hatalı girişte user blocklama

					if(result.Succeeded)
					{
						string returnUrl = login.ReturnUrl ?? "";
						if(!string.IsNullOrEmpty(returnUrl))
						{	
							string[] returnUrlArray = returnUrl.Split('/');
							return RedirectToAction(returnUrlArray[2], returnUrlArray[1]);
						}
					}
						//return RedirectToAction(login.ReturnUrl ?? "/");
				}

				ModelState.AddModelError(nameof(login.Email), "Login failed: Invalid email or password!");
			}

			return View(login);
		}

		public IActionResult AccessDenied()
		{
			return View("AccessDenied");
		}

		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}
	}
}
