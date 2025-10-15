using JobHuntAPI.Model.Dto;
using JobHuntAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JobHuntAPI.Controllers
{

	[Authorize]
	[ApiController]
	[Route("[controller]")]
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

		[HttpPost("new/multiple")]
		public async Task<IActionResult> AddMultipleNew([FromBody] IEnumerable<JobApplicationDto> dtos)
		{
			try
			{
				await _applicationService.AddNewAsync(dtos);
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

		[HttpGet("all")]
		public async Task<IActionResult> GetAllForUser([FromQuery] Guid userId)
		{
			try
			{
				var items = await _applicationService.GetAllAsync<JobApplicationDisplayDto>(userId);
				if (items == null || !items.Any())
					return NotFound(new { error = "No applications found for this user." });

				return Ok(items);
			}
			catch (ArgumentException ex)
			{
				return BadRequest(new { error = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
			}
		}

		[HttpDelete("delete")]
		public async Task<IActionResult> Delete([FromQuery]Guid userId, [FromQuery]Guid applicationId)
		{
			try
			{
				await _applicationService.DeleteByIdAsync(userId, applicationId);
				return Ok();
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

		[HttpDelete("delete/multiple")]
		public async Task<IActionResult> DeleteMultiple([FromQuery] Guid userId, [FromBody] IEnumerable<Guid> applicationIds)
		{
			try
			{
				await _applicationService.DeleteByIdAsync(userId, applicationIds);
				return Ok();
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

		[HttpPatch("update")]
		public async Task<IActionResult> Update([FromQuery] Guid userId, [FromBody] JobApplicationDisplayDto dto)
		{
			try
			{
				await _applicationService.UpdateApplicationAsync(userId, dto);
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

		[HttpPatch("update/multiple")]
		public async Task<IActionResult> UpdateMultiple([FromQuery] Guid userId, [FromBody] IEnumerable<JobApplicationDisplayDto> dtos)
		{
			try
			{
				await _applicationService.UpdateApplicationAsync(userId, dtos);
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
			Log.Warning("API out here");
			return "accessed";
		}
	}
}
