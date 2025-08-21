using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHuntAPI.Model
{
	[Table("Users")]
	public class User
	{
		[Key]
		public Guid UserId { get; set; }

		[Required]
		public string UserName { get; set; } = string.Empty;

		[Required]
		public string HashedPassword { get; set; } = string.Empty;

		public List<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
	}
}
