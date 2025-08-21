using System.Security.Cryptography;
using System.Text;

namespace JobHuntAPI.Utility
{
	public class EncryptionHandler
	{



		public static string GetRandomSalt(int size = 32)
		{
			byte[] salt = new byte[size];
			RandomNumberGenerator.Create().GetBytes(salt, 0, salt.Length);
			return Convert.ToBase64String(salt);
		}


		public static string HashPassword(string password, string salt)
		{
			string combinedPassword = string.Concat(password, salt);

			byte[] bytes = UTF8Encoding.UTF8.GetBytes(combinedPassword);

			byte[] hash = SHA256.HashData(bytes);


			return Convert.ToBase64String(hash);
		}


		public static string Base64Encode(string plaintext)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plaintext);
			return Convert.ToBase64String(plainTextBytes);
		}
		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
			return Encoding.UTF8.GetString(base64EncodedBytes);
		}
	}
}
