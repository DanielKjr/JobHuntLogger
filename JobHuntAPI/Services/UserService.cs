using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Repository;

namespace JobHuntAPI.Services
{
	public class UserService(IRepository<UserContext> userRepository)
	{

		public List<User> GetUsers()
		{
			return userRepository.GetAllItems<User>();
		}
	}
}
