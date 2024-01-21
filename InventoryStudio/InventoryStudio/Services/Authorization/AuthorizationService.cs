using InventoryStudio.Models;
using InventoryStudio.Models.Authorization;
using ISLibrary;
using ISLibrary.AspNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryStudio.Services.Authorization
{
    public interface IAuthorizationService
    {
        Task<AuthorizationResponse> GenerateTokenByRole(string roleId);

        Task<AuthorizationResponse> GenerateTokenByUser(AuthorizationRequest request);

        Task ValidateToken(TokenValidatedContext context);
    }
    public class AuthorizationService : IAuthorizationService
    {

        private IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthorizationService(IConfiguration config, UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public async Task<AuthorizationResponse> GenerateTokenByRole(string roleId)
        {
            var role = new AspNetRoles(roleId);
            if (role == null)
                return new AuthorizationResponse { IsSuccess = false, Message = "Role does not exsits" };
            var tokenId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti,tokenId),
                    new Claim(ClaimTypes.Role,role.Name)
            };

            var tokenConfigSection = _config.GetSection("Authentication:Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigSection["Key"]));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: tokenConfigSection["Issuer"],
                audience: tokenConfigSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddYears(100),
                signingCredentials: signCredential
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return new AuthorizationResponse { IsSuccess = true, Token = token, Message = "Ok", TokenId = tokenId };
        }

        public async Task<AuthorizationResponse> GenerateTokenByUser(AuthorizationRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new AuthorizationResponse { IsSuccess = false, Message = "User already exists!" };

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);

            if (!signInResult.Succeeded)
                return new AuthorizationResponse { IsSuccess = false, Message = "Check Email or Password" };
            var tokenId = Guid.NewGuid().ToString();
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,tokenId)

                };
            var userClaims = await _userManager.GetClaimsAsync(user);
            if (userClaims.Any())
            {
                foreach (var claim in userClaims)
                {
                    claims.Add(claim);
                }
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }
            var tokenConfigSection = _config.GetSection("Security:Token");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfigSection["Key"]));
            var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(
                issuer: tokenConfigSection["Issuer"],
                audience: tokenConfigSection["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: signCredential
                );
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var expiration = TimeZoneInfo.ConvertTimeFromUtc(jwtToken.ValidTo, TimeZoneInfo.Local);
            return new AuthorizationResponse { IsSuccess = true, Token = token, Expiration = expiration, Message = "Ok", TokenId = tokenId };
        }

        public async Task ValidateToken(TokenValidatedContext context)
        {
            var accessTokenId = context.Principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var accessToken = new AspNetAccessTokens(accessTokenId);
            if (accessToken == null || !accessToken.InActive || context.SecurityToken == null)
            {
                context.Fail("Token validation failed");
                return;
            }
            var role = await _roleManager.FindByIdAsync(accessToken.RoleId);
            if (role == null)
            {
                context.Fail("Role does not exist");
                return;
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            if (roleClaims.Any())
            {
                foreach (var claim in roleClaims)
                {
                    context.Principal.AddIdentity(new ClaimsIdentity(new[] { new Claim(claim.Type, claim.Value) }));
                }
            }
            context.Success();
        }
    }
}
