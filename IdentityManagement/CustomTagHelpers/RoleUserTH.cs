using IdentityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace IdentityManagement.CustomTagHelpers
{
	[HtmlTargetElement("td", Attributes = "i-role")]
	public class RoleUserTH : TagHelper
	{
		// Yine role ve user'lara erişeceğim
		private UserManager<AppUser> userManager;
		private RoleManager<IdentityRole> roleManager;

		public RoleUserTH(UserManager<AppUser> _userManager, RoleManager<IdentityRole> _roleManager)
		{
			this.userManager = _userManager;
			this.roleManager = _roleManager;
		}

		[HtmlAttributeName("i-role")]
		public string Role { get; set; }

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			//return base.ProcessAsync(context, output);
			List<string> names = new List<string>();
			IdentityRole role = await roleManager.FindByIdAsync(Role);

			if(role != null)
			{
				foreach (var user in userManager.Users)
				{
					if (user != null && await userManager.IsInRoleAsync(user, role.Name)/*Bu user bu rolde mi*/)
						names.Add(user.UserName);
				}
			}

			output.Content.SetContent(names.Count == 0 ? "No users" : string.Join(", ", names));
		}
	}
}
