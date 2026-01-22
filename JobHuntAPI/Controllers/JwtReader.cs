using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class JwtReader : Controller
	{
		[HttpPost("/jwt")]
		public async Task<IActionResult> Decode(string jwt)
		{
			JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
			return  Ok(handler.ReadJwtToken(jwt));
		}
	}
}
