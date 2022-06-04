using System.IdentityModel.Tokens.Jwt;

public class JwtService 
{
    public JwtPayload? GetPayload(string token) 
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(token);
        var payload = jwtSecurityToken.Payload;
        return payload;
    }
}