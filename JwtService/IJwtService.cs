using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace JwtService;

public interface IJwtService
{
	/// <returns>
	/// A token generated with the passed <paramref name="descriptor"/>.
	/// </returns>
	public string CreateToken(SecurityTokenDescriptor descriptor);

	/// <returns>
	/// A generated token with the passed <paramref name="claims"/>.
	/// </returns>
	public string CreateToken(IEnumerable<Claim> claims, TimeSpan lifespan);

	/// <returns>
	/// Retrieved <seealso cref="TokenValidationResult"/>.
	/// </returns>
	public TokenValidationResult ValidateToken(string token);

}
