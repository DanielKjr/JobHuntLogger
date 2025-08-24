using JobHuntAPI.Model.Dto;
using JobHuntAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{
	[Route("authenticate")]
	[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public class AuthenticationController(AuthenticationService authenticationService) : ControllerBase
	{
		[HttpPost("create")]
		public async Task<ActionResult<bool>> CreateUser([FromBody] UserDto user)
		{
			try
			{
				var newUser = await authenticationService.CreateUser(user);
				return CreatedAtAction(nameof(CreateUser), Ok(true));
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
		}



		[HttpPost("login")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<bool>> Login([FromBody] UserDto userDto)
		{
			bool success = await authenticationService.Login(userDto);

			if (success)
				return Ok(true);
			else
				return Unauthorized();
		}





	}
}
