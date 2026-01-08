using System.Security.Claims;

namespace JobHuntAPI.Utility
{
	public interface IUserHelper
	{
		public Guid GetUserId(ClaimsPrincipal claim);
	}
}
