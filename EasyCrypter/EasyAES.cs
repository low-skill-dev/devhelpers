using Random = System.Security.Cryptography.RandomNumberGenerator;
using System.Security.Cryptography;
using System.Text;

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

	private static List<ReadOnlyMemory<char>> FastSplit(this string s, char delimiter)
	{
		List<ReadOnlyMemory<char>> ret = new(3);

		int prev = 0;
		for(int i = 0; i < s.Length; i++)
		{
			if(s[i] == delimiter || i == s.Length - 1)
			{
				if(i - prev <= 0) continue;
				if(i == s.Length - 1) i++;

				ret.Add(s.AsMemory().Slice(prev, i - prev));
				prev = i + 1;
			}
		}

		return ret;
	}


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

	public static string EncryptString(string plain, string key)
	{
		var result = Encrypt(Encoding.UTF8.GetBytes(plain), Encoding.UTF8.GetBytes(key));
		var cipher = Convert.ToHexString(result.Cipher);
		var nonce = Convert.ToHexString(result.Nonce);
		var tag = Convert.ToHexString(result.Tag);

		var sb = new StringBuilder(cipher.Length + nonce.Length + tag.Length);

		sb.Append(Convert.ToHexString(result.Cipher));
		sb.Append('_');
		sb.Append(Convert.ToHexString(result.Nonce));
		sb.Append('_');
		sb.Append(Convert.ToHexString(result.Tag));

		return sb.ToString();
	}
	public static string DecryptString(string encrypted, string key)
	{
		var parts = encrypted.FastSplit('_');
		var cipher = Convert.FromHexString(parts[0].Span);
		var nonce = Convert.FromHexString(parts[1].Span);
		var tag = Convert.FromHexString(parts[2].Span);

		var result = Decrypt(new AesEncryptionResult()
		{
			Cipher = cipher,
			Nonce = nonce,
			Tag = tag,
		}, Encoding.UTF8.GetBytes(key));

		return Encoding.UTF8.GetString(result);
	}



}