using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace JwtService;

public static class Extensions
{
	public static DateTime GetIat(this TokenValidationResult t)
	{
		return DateTimeOffset.FromUnixTimeSeconds(
			(long)t.Claims[JwtRegisteredClaimNames.Iat])
			.UtcDateTime;
	}
	public static DateTime GetNbf(this TokenValidationResult t)
	{
		return DateTimeOffset.FromUnixTimeSeconds(
			(long)t.Claims[JwtRegisteredClaimNames.Nbf])
			.UtcDateTime;
	}
	public static DateTime GetExp(this TokenValidationResult t)
	{
		return DateTimeOffset.FromUnixTimeSeconds(
			(long)t.Claims[JwtRegisteredClaimNames.Exp])
			.UtcDateTime;
	}
	public static string ToStringRepresentation(this IEnumerable<Claim> claims)
	{
		var joined = string.Join(',', claims.Select(x => $"'{x.Type}':'{x.ValueType}':'{x.Value}'"));
		return $"[{joined}]";
	}
}