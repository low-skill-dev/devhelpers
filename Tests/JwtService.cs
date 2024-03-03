using JwtService;
using Microsoft.IdentityModel.JsonWebTokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Tests;

public class JwtService
{

	private IJwtService _service;

	public JwtService()
	{
		var ecdsa = ECDsa.Create();
		ecdsa.GenerateKey(ECCurve.NamedCurves.nistP521);

		_service = new EcdsaJwtService(ECDsa.Create(), null);
	}

	[Fact]
	public async Task CanIssueAndValidate()
	{
		var guid = Guid.NewGuid().ToString();
		var jti = guid.Substring(31);
		var randomData = Random.Shared.NextInt64().ToString();

		var claims = new Claim[]
		{
			new(JwtRegisteredClaimNames.Jti, jti),
			new(JwtRegisteredClaimNames.Sub, guid),
			new("mydata", randomData),
		};

		var token = _service.CreateToken(claims, TimeSpan.FromSeconds(5));

		Assert.NotEmpty(token);
		Assert.NotNull(_service.ValidateToken(token));
		Assert.True(_service.ValidateToken(token).IsValid);

		await Task.Delay(TimeSpan.FromSeconds(3));
		Assert.True(_service.ValidateToken(token).IsValid);

		await Task.Delay(TimeSpan.FromSeconds(1));
		Assert.True(_service.ValidateToken(token).IsValid);

		await Task.Delay(TimeSpan.FromSeconds(1));
		Assert.False(_service.ValidateToken(token).IsValid);
	}
}
