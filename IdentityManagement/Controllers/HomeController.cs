using System.Diagnostics;
using IdentityManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private UserManager<AppUser> userManager;

		public HomeController(ILogger<HomeController> logger, UserManager<AppUser> _userManager)
		{
			_logger = logger;
			userManager = _userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		// Credentials'ý doðrulanmýþ kiþilerin göreceði sayfa
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Secured()
		{
			AppUser user = await userManager.GetUserAsync(HttpContext.User);
			string message = "Hello, " + user.UserName;
			return View((object)message);
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
