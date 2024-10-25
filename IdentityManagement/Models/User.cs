using System.ComponentModel.DataAnnotations;

namespace IdentityManagement.Models
{
    public class User
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Email is not valid!")] 
        // EmailAddress de kullanabilirsin
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
		public Country Country { get; set; }
		public int Age { get; set; }

		[Required]
		public string Salary { get; set; }
	}
}
