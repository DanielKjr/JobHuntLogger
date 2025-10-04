using JobHuntAPI.Model.Dto;
using JobHuntAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{

	[ApiController]
	[Route("api/[controller]")]
	public class ApplicationController(IApplicationService _applicationService) : ControllerBase
	{

		[HttpPost("new")]
		public async Task<IActionResult> AddNew([FromBody] JobApplicationDto dto)
		{
			try
			{
				await _applicationService.AddNewAsync(dto);
				return Ok();
			}
			catch (ArgumentNullException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { error = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
			}
		}

		[HttpGet("temp")]
		public string Temp()
		{
			return "accessed";
		}
	}
}
