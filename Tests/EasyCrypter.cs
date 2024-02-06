using EasyCrypter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

public class EasyCrypter
{
	[Fact]
	public static void CanEncryptAndDecryptSmall()
	{
		var text = "What is Lorem Ipsum?";
		var bytes = Encoding.UTF8.GetBytes(text);
		var key = RandomNumberGenerator.GetBytes(
			RandomNumberGenerator.GetInt32(16, short.MaxValue));

		var encrypted = EasyAES.Encrypt(bytes, key);
		var decrypted = EasyAES.Decrypt(encrypted, key);
		var newText = Encoding.UTF8.GetString(decrypted);

		Assert.Equal(text, newText);
	}

	[Fact]
	public static async Task CanEncryptAndDecryptLarge()
	{
		var client = new HttpClient();
		var text = await client.GetStringAsync("https://www.lipsum.com/");
		var bytes = Encoding.UTF8.GetBytes(text);
		var key = RandomNumberGenerator.GetBytes(
			RandomNumberGenerator.GetInt32(16, short.MaxValue));

		var encrypted = EasyAES.Encrypt(bytes, key);
		var decrypted = EasyAES.Decrypt(encrypted, key);
		var newText = Encoding.UTF8.GetString(decrypted);

		Assert.Equal(text, newText);
	}
}
