using DK.GenericLibrary.Interfaces;
using JobHuntAPI.Model;
using JobHuntAPI.Model.Dto;
using JobHuntAPI.Repository;
using Microsoft.AspNetCore.Identity;

namespace JobHuntAPI.Services
{
	public class LoginService(IAsyncRepository<UserContext> userRepository)
	{
		
		public async Task<bool> Login(UserDto userDto)
		{
			User user = await userRepository.GetItem<User>(q =>
				q.Where(i => i.UserName == userDto.UserName));

			if (user == null)
				return false; // username not found

			PasswordHasher<User> hasher = new PasswordHasher<User>();
			var result = hasher.VerifyHashedPassword(user, user.HashedPassword, userDto.Password);

			return result == PasswordVerificationResult.Success;
		}

		public async Task<User> CreateUser(UserDto userDto)
		{
			User existingUser = await userRepository.GetItem<User>(q => q.Where(i => i.UserName == userDto.UserName));
			if (existingUser != null)
				throw new InvalidOperationException("Username already exists.");

			User newUser = new User()
			{
				UserName = userDto.UserName
			};

			//uses PBKDF2-HMAC-SHA256
			var hasher = new PasswordHasher<User>();
			newUser.HashedPassword = hasher.HashPassword(newUser, userDto.Password);

			await userRepository.AddItem(newUser);
			return newUser;
		}
	}
}

