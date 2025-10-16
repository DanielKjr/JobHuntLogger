namespace JobHuntAPI.Model.Dto
{
	public class NewJobApplicationDto
	{
		public string JobTitle { get; set; }
		public string Company { get; set; }
		public PdfFile ApplicationPdf { get; set; }
		public PdfFile ResumePdf { get; set; }
	}
}
