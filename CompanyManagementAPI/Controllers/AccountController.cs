
using CompanyManagementAPI.DTOs;
using CompanyManagementAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CompanyManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _usermanger;
        private readonly IConfiguration _config;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _usermanger = userManager;
            _config = configuration;
        }

        // Register
        [HttpPost("register")]
        public async Task<IActionResult> Registeration(RegisterUserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser();

                applicationUser.UserName = userDTO.UserName;
                applicationUser.Email = userDTO.Email;

                IdentityResult result = await _usermanger.CreateAsync(
                    applicationUser,
                    userDTO.Password!
                );

                if (result.Succeeded)
                {
                    return Ok("User Account Added.");
                }
                else
                {
                    return BadRequest(result.Errors.FirstOrDefault());
                }
            }

            return BadRequest(ModelState);
        }

        // Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO userDto)
        {
            if (ModelState.IsValid)
            {
                // Check User
                ApplicationUser? user =
                    await _usermanger.FindByNameAsync(userDto.UserName!);

                if (user != null)
                {
                    bool found = await _usermanger.CheckPasswordAsync(
                        user,
                        userDto.Password!
                    );

                    if (found)
                    {
                        // Claims
                        List<Claim> claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Name, user.UserName!));

                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));

                        claims.Add(new Claim(
                            JwtRegisteredClaimNames.Jti,
                            Guid.NewGuid().ToString()
                        ));

                        // Roles
                        var roles = await _usermanger.GetRolesAsync(user);

                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        // Signing Credentials
                        SecurityKey securityKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                _config["Jwt:SecurityKey"]!
                            )
                        );

                        SigningCredentials signingCredentials =
                            new SigningCredentials(
                                securityKey,
                                SecurityAlgorithms.HmacSha256
                            );

                        // Create Token
                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            issuer: _config["Jwt:Issuer"],
                            audience: _config["Jwt:Audience"],
                            claims: claims,
                            expires: DateTime.UtcNow.AddMinutes(30),
                            signingCredentials: signingCredentials
                        );

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler()
                                .WriteToken(mytoken),

                            expiration = mytoken.ValidTo
                        });
                    }
                }
            }

            return Unauthorized();
        }
    }
}
