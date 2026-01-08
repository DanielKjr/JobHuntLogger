using System.ComponentModel.DataAnnotations;

namespace JobHuntAPI.Model.Dto
{
	public class NewJobApplicationDto
	{

		[MinLength(3), MaxLength(100)]
		public required string JobTitle { get; set; }
		[MinLength(3), MaxLength(100)]
		public required string Company { get; set; }

		public PdfFileDto? ApplicationPdf { get; set; }
		public PdfFileDto? ResumePdf { get; set; }

		public DateTime Date { get; set; }
		public DateTime Deadline { get; set; }
	}
}
