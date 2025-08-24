using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace JobHuntAPI.Model
{
	[Table("Users")]
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid UserId { get; set; }

		[Required]
		[MaxLength(50)]
		public string UserName { get; set; } = string.Empty;

		[Required]
		[MaxLength(512)]
		public string HashedPassword { get; set; } = string.Empty;

		public List<JobApplication> JobApplications { get; set; } = new List<JobApplication>();


	}
}
