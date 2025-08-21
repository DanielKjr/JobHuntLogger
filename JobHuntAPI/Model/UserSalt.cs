using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobHuntAPI.Model
{
	[Table("UserSalts")]
	public class UserSalt
	{

		[Key]
		public Guid UserSaltId { get; set; }

		[Required]
		public string Salt { get; set; } = string.Empty;

		[Required]
		public User? User { get; set; }
	}
}
