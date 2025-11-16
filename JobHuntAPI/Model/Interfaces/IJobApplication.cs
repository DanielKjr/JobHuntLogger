namespace JobHuntAPI.Model.Interfaces
{
	public interface IJobApplication
	{
		Guid UserId { get; set; }
		Guid JobApplicationId { get; set; }
		string Company { get; set; }
		string JobTitle { get; set; }
		DateTime AppliedDate { get; set; }
		DateTime ReplyDate { get; set; }

		ICollection<PdfFile> PdfFiles { get; set; }
	
	}
}