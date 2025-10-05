namespace JobHuntLogger.Services.Authorization
{
	public interface ITokenProvider
	{
		Task<string> GetTokenAsync();
	}
}
