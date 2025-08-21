using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHuntAPI.Model
{
	[Table("Applications")]
	public class JobApplication
	{

		[Key]
		public Guid JobApplicationId { get; set; }

		[Required]
		public Guid UserId { get; set; }

		[Required]
		public string JobTitle { get; set; } = string.Empty;
		[Required]
		public string Company { get; set; } = string.Empty;


		public DateTime AppliedDate { get; set; }

		public DateTime ReplyDate { get; set; }

		//should probably be encrypted
		public string Base64Application { get; set; } = string.Empty;

		public string Base64Resume { get; set; } = string.Empty;

		public JobApplication()
		{
			AppliedDate = DateTime.Now;
		}

	}
}
