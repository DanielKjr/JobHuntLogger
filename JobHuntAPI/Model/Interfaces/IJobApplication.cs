namespace JobHuntAPI.Model.Interfaces
{
	public interface IJobApplication
	{
		DateTime AppliedDate { get; set; }
		string Company { get; set; }
		byte[] EncryptedApplicationPdf { get; set; }
		byte[] EncryptedResumePdf { get; set; }
		Guid JobApplicationId { get; set; }
		string JobTitle { get; set; }
		DateTime ReplyDate { get; set; }
		Guid UserId { get; set; }
	}
}