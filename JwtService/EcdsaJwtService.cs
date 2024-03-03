using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JwtService;

public sealed class EcdsaJwtService : IJwtService
{
	#region fields, props, ctors

	private static readonly ECCurve Curve = ECCurve.NamedCurves.nistP521;
	private static readonly string Algorithm = SecurityAlgorithms.EcdsaSha512Signature;

	private readonly ILogger<EcdsaJwtService>? _logger;
	private readonly JsonWebTokenHandler _tokenHandler;
	private readonly ECDsaSecurityKey _securityKey;

	public EcdsaJwtService(string signingEcdsaPem, ILogger<EcdsaJwtService>? logger)
		: this(GetEcdsaFromPem(signingEcdsaPem), logger) { }
	public EcdsaJwtService(ECDsa signingCert, ILogger<EcdsaJwtService>? logger)
		: this(new ECDsaSecurityKey(signingCert), logger) { }
	public EcdsaJwtService(ECDsaSecurityKey securityKey, ILogger<EcdsaJwtService>? logger)
	{
		_tokenHandler = new JsonWebTokenHandler();
		_securityKey = securityKey;
		_logger = logger;
	}

	#endregion

	#region IJwtService

	public string CreateToken(SecurityTokenDescriptor descriptor)
	{
		return _tokenHandler.CreateToken(descriptor);
	}
	public string CreateToken(IEnumerable<Claim> claims, TimeSpan lifespan)
	{
		return CreateToken(new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.Add(lifespan),
			NotBefore = DateTime.UtcNow.AddSeconds(-1),
			SigningCredentials = new SigningCredentials(
				_securityKey, Algorithm),
		});
	}

	public TokenValidationResult ValidateToken(string token)
	{
		return _tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = _securityKey,
			ClockSkew = TimeSpan.Zero
			/* Clock skew in JWT (JSON Web Token) refers to the difference in time between the clock 
			 * on the server that issued the token and the clock on the server that is verifying the token. 
			 * This difference can cause issues with token validation, as the token may appear to be 
			 * expired even though it is still valid according to the issuing server's clock. 
			 * To account for clock skew, JWT implementations typically allow for a certain amount of 
			 * leeway in the token's expiration time, or include a timestamp in the token itself that 
			 * can be used to calculate the actual expiration time regardless of clock differences. 
			 * It is important to properly handle clock skew to ensure that JWT-based authentication 
			 * and authorization systems are reliable and secure.
			 * https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/dev/src/Microsoft.IdentityModel.Tokens/TokenValidationParameters.cs#L345
			 */
		}).Result;
	}

	#endregion

	private static ECDsa GetEcdsaFromPem(string ecdsaPem)
	{
		var ecdsa = ECDsa.Create();
		ecdsa.ImportFromPem(ecdsaPem);
		return ecdsa;
	}
}