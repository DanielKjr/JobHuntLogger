namespace JobHuntLogger.Components.Model.Authentication
{

	public class UserSession
	{
		public event Action? OnChange;

		private bool isLoggedIn;
		public bool IsLoggedin
		{
			get => isLoggedIn;
			set
			{
				if (isLoggedIn != value)
				{
					isLoggedIn = value;
					NotifyStateChanged();
				}
			}

		}

		public string UserName { get; set; } = string.Empty;
		public string Role { get; set; } = string.Empty;

		private void NotifyStateChanged() => OnChange?.Invoke();
	}
}
