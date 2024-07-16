using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace InntalerSchachfreunde.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(HttpClient httpClient, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<AuthResponse?> Login(AuthRequest request)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(request.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, request.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = CreateToken(authClaims);
                var authResponse = new AuthResponse()
                {
                    Username = request.Username,
                    Token = token,
                    Email = user.Email,
                    Roles = userRoles.ToList(),
                    UserId = user.Id
                };
                return authResponse;
            }
            else
            {
                return null;
            }
        }

        private string CreateToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                    _configuration.GetSection("AppSettings:Token").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                                   claims: claims,
                                   expires: DateTime.UtcNow.AddDays(1),
                                   signingCredentials: cred
   );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public async Task<bool> Register(RegisterRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("auth/register", request);
            return result.IsSuccessStatusCode;
        }
    }
    public class AuthRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }

    public record AuthResponse
    {
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
    }

    public class RegisterRequest
    {
        public string? FullName { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string[]? Roles { get; set; }
    }
}
