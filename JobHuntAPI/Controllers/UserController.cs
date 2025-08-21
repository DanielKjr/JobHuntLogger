using JobHuntAPI.Model;
using JobHuntAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{
	[Route("user")]
	public class UserController(UserService userService) : ControllerBase
	{

		[HttpGet("getall")]
		public List<User> GetUsers()
		{
			return userService.GetUsers();
		}
	}
}
