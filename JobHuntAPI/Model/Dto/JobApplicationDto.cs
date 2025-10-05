using System.ComponentModel.DataAnnotations;

namespace JobHuntAPI.Model.Dto
{
	public class JobApplicationDto 
	{
		public required Guid UserId { get; set; }
		public required string JobTitle { get; set; }
		public required string Company { get; set; }
		public required byte[] ApplicationPdf { get; set; }
		public required byte[] ResumePdf { get; set; }
	}
}
