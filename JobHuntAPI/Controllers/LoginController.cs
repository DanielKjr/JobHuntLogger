using JobHuntAPI.Model.Dto;
using JobHuntAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{
	/// <summary>
	/// Controller called by the AuthorizationServer, passing on credentials
	/// </summary>
	/// <param name="loginService"></param>
	[Route("authenticate")]
	public class LoginController(LoginService loginService) : ControllerBase
	{
	


		[HttpPost("login")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<bool>> Login([FromBody] UserDto userDto)
		{
			bool success = await loginService.Login(userDto);

			if (success)
				return Ok(true);
			else
				return Unauthorized();
		}

		[HttpPost("create")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<bool>> CreateUser([FromBody] UserDto user)
		{
			try
			{
				var newUser = await loginService.CreateUser(user);
				return CreatedAtAction(nameof(CreateUser),null , true);
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}

	}
}
