using System.ComponentModel.DataAnnotations;

namespace JobHuntLogger.Model
{
	public class AccountCreation
	{
		[Required]
		[MinLength(5, ErrorMessage = "Name must be longer than 5characters.")]
		[MaxLength(32)]
		public string Name { get; set; } = string.Empty;


		[Required]
		[MinLength(5, ErrorMessage = "Password must be longer than 5 characters.")]
		[MaxLength(30, ErrorMessage = "Password can not be longer than 30 characters.")]
		public string Password { get; set; } = string.Empty;
		[Required]
		[Compare("Password", ErrorMessage = "The Passwords does not match.")]
		public string ConfirmPassword { get; set; } = string.Empty;




	}
}
