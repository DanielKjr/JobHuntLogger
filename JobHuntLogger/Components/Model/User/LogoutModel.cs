﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JobHuntLogger.Components.Model.User
{
	public class LogoutModel : PageModel
	{
		public IActionResult OnGet()
		{
			return SignOut(
				new AuthenticationProperties { RedirectUri = "/" },
				OpenIdConnectDefaults.AuthenticationScheme,
				CookieAuthenticationDefaults.AuthenticationScheme);
		}
	}
}
