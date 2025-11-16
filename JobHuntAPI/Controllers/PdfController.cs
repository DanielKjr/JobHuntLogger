using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobHuntAPI.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class PdfController(IPdfService _pdfService) : ControllerBase
	{
		[HttpPost("/pdf")]
		[ProducesResponseType(typeof(PdfFile), 200)]
		public IActionResult GetPdf([FromBody] PdfRequestDto dto)
		{
			var pdf =  _pdfService.GetPdf(dto);
			//Entire controller may be temporary, it was made to return a File but that didn't solve my problem.
			return Ok(pdf);
		}

	}
}
