using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;

namespace JobHuntAPI.Services.Interfaces
{
	public interface IPdfService
	{
		PdfFile GetPdf(PdfRequestDto dto);
	}
}
