using JobHuntAPI.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHuntAPI.Model
{
	[Table("Applications")]
	public class JobApplication : IJobApplication
	{

		[Key]
		public Guid JobApplicationId { get; set; }

		[ForeignKey("UserId")]
		public Guid UserId { get; set; }

		[Required]
		public string JobTitle { get; set; } = string.Empty;
		[Required]
		public string Company { get; set; } = string.Empty;


		public DateTime AppliedDate { get; set; }

		public DateTime ReplyDate { get; set; }

		
		public PdfFile EncryptedApplicationPdf { get; set; } 

		public PdfFile EncryptedResumePdf { get; set; } 

		public JobApplication()
		{
			AppliedDate = DateTime.Now;
		}

	}
}
