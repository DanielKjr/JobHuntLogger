using System.Security.Cryptography;

namespace JobHuntAPI.Services
{
	public class PDFEncryptionHelper
	{
		public static byte[] EncryptPdf(byte[] pdfBytes, string userId, string secret)
		{
			using var aes = Aes.Create();
			var key = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(userId + secret));
			aes.Key = key;
			aes.GenerateIV();
			using var encryptor = aes.CreateEncryptor();
			var encrypted = encryptor.TransformFinalBlock(pdfBytes, 0, pdfBytes.Length);
			
			return aes.IV.Concat(encrypted).ToArray();
		}

		public static byte[] DecryptPdf(byte[] encryptedPdf, string userId, string secret)
		{
			using var aes = Aes.Create();
			var key = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(userId + secret));
			aes.Key = key;
			var iv = encryptedPdf.Take(aes.BlockSize / 8).ToArray();
			var data = encryptedPdf.Skip(aes.BlockSize / 8).ToArray();
			aes.IV = iv;
			using var decryptor = aes.CreateDecryptor();
			return decryptor.TransformFinalBlock(data, 0, data.Length);
		}

		public static bool IsFileChanged(byte[] newFile, byte[] existingFile)
		{
			if (newFile == null && existingFile == null) return false;
			if (newFile == null || existingFile == null) return true;
			if (newFile.Length != existingFile.Length) return true;
			for (int i = 0; i < newFile.Length; i++)
				if (newFile[i] != existingFile[i]) return true;
			return false;
		}
	}
}
