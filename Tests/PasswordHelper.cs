using PasswordHelper;
using System.Security.Cryptography;
using Xunit;

namespace Tests;

public class PasswordHelper
{
	[Fact]
	public void CanHashAndConfirm()
	{
		var password = Convert.ToBase64String(RandomNumberGenerator.GetBytes(Random.Shared.Next(1, 129)));

		var hashed = PasswordHasher.HashPassword(password, out var salt);

		Assert.True(PasswordHasher.ConfirmPassword(password,
			Convert.FromBase64String(Convert.ToBase64String(hashed)),
			Convert.FromBase64String(Convert.ToBase64String(salt))));
	}
}
