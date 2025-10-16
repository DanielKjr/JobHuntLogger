namespace JobHuntAPI.Model.Dto
{
	public class JobApplicationDisplayDto
	{
		public required Guid JobApplicationId { get; set; }
		public required string JobTitle { get; set; }
		public required string Company { get; set; }
		public required PdfFile ApplicationPdf { get; set; }
		public required PdfFile ResumePdf { get; set; }
		public DateTime AppliedDate { get; set; }
		public DateTime ReplyDate { get; set; }

	}
}
