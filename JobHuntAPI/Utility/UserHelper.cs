
using System.Security.Claims;

namespace JobHuntAPI.Utility
{
	public class UserHelper() : IUserHelper
	{
		public Guid GetUserId(ClaimsPrincipal claim)
		{
			return (Guid.Parse(claim.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")!));
		}
	}
}
