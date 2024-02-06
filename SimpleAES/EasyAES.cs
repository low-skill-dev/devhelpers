using Random = System.Security.Cryptography.RandomNumberGenerator;
using System.Security.Cryptography;

namespace EasyCrypter;


public static class EasyAES
{
	public class AesEncryptionResult
	{
		public required byte[] Cipher;
		public required byte[] Nonce;
		public required byte[] Tag;
	}

	private const int keySizeInBytes = 256 / 8;
	private const int tagSizeInBytes = 128 / 8;
	private const int blockSizeBytes = 128 / 8;
	private const int nonceSizeInBytes = 96 / 8;
	private const int HexToBytesLengthRatio = 2 / 1;


	public static AesEncryptionResult Encrypt(ReadOnlySpan<byte> plain, ReadOnlySpan<byte> key)
	{
		var aes = new AesGcm(SHA256.HashData(key), tagSizeInBytes);

		var nonce = Random.GetBytes(nonceSizeInBytes);
		var cipher = new byte[plain.Length];
		var tag = new byte[tagSizeInBytes];

		aes.Encrypt(nonce, plain, cipher, tag);

		return new()
		{
			Cipher = cipher,
			Nonce = nonce,
			Tag = tag,
		};
	}

	public static byte[] Decrypt(AesEncryptionResult encrypted, ReadOnlySpan<byte> key)
	{
		var aes = new AesGcm(SHA256.HashData(key), tagSizeInBytes);
		var result = new byte[encrypted.Cipher.Length];

		aes.Decrypt(encrypted.Nonce, encrypted.Cipher, encrypted.Tag, result);

		return result;
	}


	//private static void Utf8HexToBytes(ReadOnlySpan<char> hex, Span<byte> buf)
	//{
	//	for(int i = 0; i < hex.Length; i += 2)
	//		buf[i / 2] = Convert.ToByte((((0 << 4)
	//			| (hex[i] - (hex[i] <= '9' ? '0' : ('A' - 10))) << 4)
	//			| (hex[i + 1] - (hex[i + 1] <= '9' ? '0' : ('A' - 10)))));
	//}

	//public static string DecryptString(ReadOnlySpan<byte> keyBytes, string hexCipher, string hexNonce, string hexTag)
	//{
	//	Span<byte> key = SHA256.HashData(keyBytes);
	//	Span<byte> plain = Convert.FromHexString(hexCipher);
	//	Span<byte> cipher = Convert.FromHexString(hexCipher);
	//	Span<byte> nonce = Convert.FromHexString(hexNonce);
	//	Span<byte> tag = Convert.FromHexString(hexTag);

	//	var aes = new AesGcm(key, tagSizeInBytes);

	//	aes.Decrypt(nonce, cipher, tag, plain);

	//	return UTF8.GetString(plain);
	//}

	//public static string DecryptString(ReadOnlySpan<byte> keyBytes, StringEncryptionResult encrypted)
	//	=> DecryptString(keyBytes, encrypted.HexAesGcmCipher, encrypted.HexAesGcmNonce, encrypted.HexAesGcmTag);
}

