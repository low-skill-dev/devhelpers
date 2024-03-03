using System.Text;
using System.Security.Cryptography;

namespace PasswordHelper;

public static class PasswordHasher
{
	private const int SaltSizeBytes = 512 / 8;

	private static byte[] ConcatBytes(byte[] first, byte[] second)
	{
		var result = new byte[first.Length + second.Length];
		first.CopyTo(result, 0);
		second.CopyTo(result, first.Length);

		return result;
	}
	private static byte[] GenerateSalt()
	{
		return RandomNumberGenerator.GetBytes(SaltSizeBytes);
	}
	private static byte[] HashPassword(string passwordPlainText, byte[] salt)
	{
		return SHA512.HashData(ConcatBytes(Encoding.UTF8.GetBytes(passwordPlainText), salt));
	}

	public static byte[] HashPassword(string passwordPlainText, out byte[] generatedSalt)
	{
		generatedSalt = GenerateSalt();
		return HashPassword(passwordPlainText, generatedSalt);
	}
	public static bool ConfirmPassword(string passwordPlainText, byte[] hashedPassword, byte[] salt)
	{
		return HashPassword(passwordPlainText, salt).SequenceEqual(hashedPassword);
	}
}
