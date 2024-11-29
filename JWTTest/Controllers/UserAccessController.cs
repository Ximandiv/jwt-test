using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTTest.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserAccessController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserModel[] userModels = [
        new UserModel("testuser", "123456789"),
        new UserModel("testadmin", "123456789")
    ];

    public UserAccessController(IConfiguration config)
    {
        _configuration = config;
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login([FromBody] UserModel user)
    {
        var testUser = userModels.Where(us => us.UserName == user.UserName).FirstOrDefault();

        // Here, you are supposed to look in DB for the user then validate user input from DB
        if(user.UserName == testUser.UserName && user.Password == testUser.Password)
        {
            var claims = new List<Claim>()
            {
                new Claim("UserId", testUser.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, testUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (user.UserName == "testadmin")
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"])),
                signingCredentials: creds
            );

            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }

        return Unauthorized();
    }
}
