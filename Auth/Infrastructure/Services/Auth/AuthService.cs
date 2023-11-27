using Auth.Domain.Entities;
using Auth.Domain.Enums;
using Auth.Infrastructure.Exceptions;
using Auth.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auth.Infrastructure.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSetting _jwt;
        private readonly IStringLocalizer<AuthService> _localization;
        private readonly IHttpContextAccessor _httpContext;

        public AuthService(IHttpContextAccessor httpContext, IStringLocalizer<AuthService> localization, UserManager<ApplicationUser> userManager, IOptions<JwtSetting> jwt)
        {
            _userManager = userManager;
            _httpContext = httpContext;
            _jwt = jwt.Value;
            _localization = localization;
        }
        public string UserId
        {
            get
            {
                return _httpContext?.HttpContext?.User?.FindFirst("uid")?.Value ?? "";
            }
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {

            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                throw new Exception(_localization["EmailAlreadyRegistered"]);


            if (await _userManager.FindByNameAsync(model.Username) is not null)
                throw new Exception(_localization["UsernameAlreadyRegistered"]);

            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ActiveCode = false,
                Code = 1234,
                CreatedAt = DateTime.Now, 
                IsActive = false,
                TypeUser = TypeUser.User
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                List<string> errors = new List<string>();

                foreach (var error in result.Errors)
                    errors.Add($"{error.Description},");

                throw new CustomValidationException(errors);
            }

            await _userManager.AddToRoleAsync(user, "User");
            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            return await GetUserAsync(user, null, jwtSecurityToken);
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new Exception(_localization["EmailPasswordIncorrect"]);


            var jwtSecurityToken = await CreateJwtToken(user);
            return await GetUserAsync(user, null, jwtSecurityToken);
        }

        public async Task<AuthModel> ForgetPasswordAsync(ForgetPasswordModel model)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    throw new Exception(_localization["UserNotFound"]);
                }
                if (user.Code != model.Code)
                {
                    throw new Exception(_localization["TheCodeIncorrect"]);
                }
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var result = await _userManager.ResetPasswordAsync(user, code, model.NewPassword);
                if (!result.Succeeded)
                {
                    throw new CustomValidationException(result.Errors.Select(x => x.Description).ToList());
                }
                return await GetUserAsync(user, null, null);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<AuthModel> ChangePasswordAsync(ChangePasswordModel model)
        {

            var userMobileExists = await _userManager.FindByEmailAsync(model.Email);

            if (userMobileExists is not null)
            {
                var user = await _userManager.GeneratePasswordResetTokenAsync(userMobileExists);
                await _userManager.ResetPasswordAsync(userMobileExists, user, model.Password);

                return await GetUserAsync(userMobileExists, _localization["PasswordUpdatedSuccessfully"], null);

            }
            throw new Exception(_localization["SonethingWrong"]);


        }

        public async Task<AuthModel> SendCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                user.Code = 12345;
                await _userManager.UpdateAsync(user);
                return await GetUserAsync(user, null, null);
            }
            throw new Exception(_localization["EmailIncorrect"]);
        }
        public async Task<AuthModel> RefreshTokenAsync(string refreshToken)
        {


            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == UserId && u.RefreshTokens.Any(t => t.Token == refreshToken));
            if (user == null)
                throw new Exception(_localization["InvalidToken"]);

            var currentRefreshToken = user.RefreshTokens.Single(t => t.Token == refreshToken);
            if (!currentRefreshToken.IsActive)
                throw new Exception(_localization["InvalidToken"]);

            currentRefreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);

            return await GetUserAsync(user, null, jwtToken);
        }
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == UserId && u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
                throw new Exception(_localization["UserNotFound"]);

            var currentRefreshToken = user.RefreshTokens?.Single(t => t.Token == refreshToken);

            if (!currentRefreshToken.IsActive)
                throw new Exception(_localization["InvalidToken"]);

            currentRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return true;

        }
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
        private async Task<AuthModel> GetUserAsync(ApplicationUser user, string? msg, JwtSecurityToken? jwtSecurityToken)
        {

            string RefreshToken = "";
            DateTime? RefreshTokenExpiration;
            var activeRefreshToken = user.RefreshTokens?.FirstOrDefault(t => t.IsActive);
            if (activeRefreshToken != null)
            {
                RefreshToken = activeRefreshToken.Token;
                RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                RefreshToken = refreshToken.Token;
                RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens?.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return new AuthModel
            {
                Email = user.Email,
                Code = user.Code,
                ExpiresOn = jwtSecurityToken?.ValidTo,
                Roles = (await _userManager.GetRolesAsync(user)).ToList(),
                Token = jwtSecurityToken != null ? new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken) : null,
                Username = user.UserName,
                Message = msg,
                RefreshToken = RefreshToken,
                RefreshTokenExpiration = RefreshTokenExpiration,
            };
        }
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
    }
}
