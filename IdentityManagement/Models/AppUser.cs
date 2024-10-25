using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IdentityManagement.Models
{
	public class AppUser : IdentityUser
	{
		// Buraya birkaç property ekle
		public Country Country { get; set; }
		public int Age { get; set; }

		[Required]
		public string Salary { get; set; }

	}
}
