using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

namespace JobHuntAPI.Model.Dto
{
	public class JobApplicationDisplayDto
	{
		public required Guid JobApplicationId { get; set; }
		public required string JobTitle { get; set; }
		public required string Company { get; set; }

		//TODO fix that json thing can't handle null values
		
		public PdfFile? ApplicationPdf { get; set; }
	
		public PdfFile? ResumePdf { get; set; }
		public DateTime AppliedDate { get; set; }
		public DateTime ReplyDate { get; set; }

	}
}
