using IdentityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityManagement.Controllers
{
	public class AdminController : Controller
	{
		private UserManager<AppUser> userManager;
		private IPasswordHasher<AppUser> passwordHasher;
		private IUserValidator<AppUser> userValidator;
		private IPasswordValidator<AppUser> passwordValidator;

		public AdminController(UserManager<AppUser> _userManager, IPasswordHasher<AppUser> _passwordHasher, IUserValidator<AppUser> _userValidator, IPasswordValidator<AppUser> _passwordValidator)
		{
			this.userManager = _userManager;
			this.passwordHasher = _passwordHasher;
			this.userValidator = _userValidator;
			this.passwordValidator = _passwordValidator;
		}

		public IActionResult Index()
		{
			return View(userManager.Users); // User listesi
		}
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(User user) // Task: asenkron olması için
		{
			if (ModelState.IsValid)
			{
				AppUser appUser = new AppUser()
				{
					UserName = user.Name,
					Email = user.Email,
					Country = user.Country,
					Age = user.Age,
					Salary = user.Salary,
				};

				// CreateAsync hem user oluşturur hem password hash'lemesi yapar
				// Password hash olarak tutulur
				IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

				// Hata alırsak
				if (result.Succeeded) return RedirectToAction("Index", "Admin");
				else
				{
					foreach (IdentityError error in result.Errors)
					{
						ModelState.AddModelError("Create_User_Error", $"{error.Code}: {error.Description}");
						// Hataların bulunduğu div'de göstermiş olacak
					}
				}
			}
			return View(user);
		}

		public async Task<IActionResult> Update(string id)
		{
			AppUser user = await userManager.FindByIdAsync(id);

			if (user != null)
				return View(user);
			else
				return RedirectToAction("Index", "Admin");
		}

		[HttpPost]
		public async Task<IActionResult> Update(string id, string email, string password, int age, string country, string salary)
		{
			AppUser user = await userManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult validEmail = null;

				if (!string.IsNullOrEmpty(email))
				{
					validEmail = await userValidator.ValidateAsync(userManager, user);
					if (validEmail.Succeeded)
						user.Email = email;
					else
						Errors(validEmail);
				}
				else
					ModelState.AddModelError("Update_User_Error", "Email cannot be empty");

				IdentityResult validPassword = null;

				if (!string.IsNullOrEmpty(password))
				{
					validPassword = await passwordValidator.ValidateAsync(userManager, user, password);
					if (validPassword.Succeeded)
						user.PasswordHash = passwordHasher.HashPassword(user, password); // hashleme yapmak için yeni bir class kullanacağım. Onu da en yukarda inject edeceğiz. Bir user alır bir de formdan gelen password parameters
					else
						Errors(validPassword);
				}
				else
					ModelState.AddModelError("Update_User_PasswordHashError", "Password cannot be empty");

				user.Age = age;

				Country updateCountry;
				Enum.TryParse(country, out updateCountry);
				user.Country = updateCountry;

				if (!string.IsNullOrEmpty(salary))
					user.Salary = salary;
				else
					ModelState.AddModelError("", "Salary cannot be empty");

				if (!string.IsNullOrEmpty(email) && 
					!string.IsNullOrEmpty(password) && 
					!string.IsNullOrEmpty(salary) && 
					validEmail.Succeeded && 
					validPassword.Succeeded)
				{
					// DB'yi update ediyorum
					IdentityResult result = await userManager.UpdateAsync(user);
					if (result.Succeeded)
						return RedirectToAction("Index", "Admin");
					else
						Errors(result);
				}
			}

			else
				ModelState.AddModelError("Update_User_Error", "User not found!");

			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(string id)
		{
			AppUser user = await userManager.FindByIdAsync(id);

			if (user != null)
			{
				IdentityResult result = await userManager.DeleteAsync(user);

				if (result.Succeeded)
					return RedirectToAction("Index", "Admin");
				else
					Errors(result);
			}
			else
				ModelState.AddModelError("", "User not found!");

			return View("Index", userManager.Users); // Index viewa gider
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
