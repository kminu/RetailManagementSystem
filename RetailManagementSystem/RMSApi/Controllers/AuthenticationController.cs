using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RMSApi.Controllers;


[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _config;
    public record AuthenticationData(string? UserName, string? Password);
    public record UserData(int Id, string FirstName, string LastName, string UserName, string Title);

    public AuthenticationController(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Get a token
    /// </summary>
    /// <remarks>
    /// This is only exposed method to request and receive token accordingly
    /// </remarks>
    /// <returns> A token</returns>
    // Expose the method for user to be authenticated
    [HttpPost("token")]
    public ActionResult<string> Authenticate([FromBody] AuthenticationData data)
    {
        // TODO: Replace this with Azure AD
        var user = ValidateCredentials(data);

        if (user is null)
        {
            return Unauthorized();
        }

        string token = GenerateToken(user);

        return Ok(token);
    }

    private string GenerateToken(UserData user)
    {
        //Encoding SecretKey
        var secretKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(
                _config.GetValue<string>("Authentication:SecretKey")));

        //Apply security algorithm to SecretKey
        var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

        //Adds required information into claim
        List<Claim> claims = new();
        claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
        claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.UserName));
        claims.Add(new(JwtRegisteredClaimNames.GivenName, user.FirstName));
        claims.Add(new(JwtRegisteredClaimNames.FamilyName, user.LastName));
        claims.Add(new("Title", user.Title));

        //You can do something like this as well
        //claims.Add(new("title", user.Title));
        //claims.Add(new(ClaimTypes.Name, user.UserName));

        //Creates token
        var token = new JwtSecurityToken(
            _config.GetValue<string>("Authentication:Issuer"),
            _config.GetValue<string>("Authentication:Audience"),
            claims,
            DateTime.UtcNow, // When this token becomes valid
            DateTime.UtcNow.AddMinutes(10), // When the token will expire
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // TODO: Replace this with Azure AD
    private UserData? ValidateCredentials(AuthenticationData data)
    {

        if (CompareValues(data.UserName, _config.GetValue<string>("TestUser:FirstId")) &&
            CompareValues(data.Password, _config.GetValue<string>("TestUser:FirstPw")))
        {
            return new UserData(1, "John", "Smith", data.UserName!, "Admin");
        }

        if (CompareValues(data.UserName, _config.GetValue<string>("TestUser:SecondId")) &&
            CompareValues(data.Password, _config.GetValue<string>("TestUser:SecondPw")))
        {
            return new UserData(2, "Alen", "Hopper", data.UserName!, "Cashier");
        }

        return null;
    }

    // ToDo: Replace this with Azure AD
    private bool CompareValues(string? actual, string expected)
    {
        if (actual is not null)
        {
            if (actual.Equals(expected))
            {
                return true;
            }
        }
        return false;
    }
}

