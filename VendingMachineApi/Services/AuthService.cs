﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using VendingMachineApi.Dtos;
using VendingMachineApi.Helpers;
using VendingMachineApi.Models;
using VendingMachineApi.Services.Interfaces;

namespace VendingMachineApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        public AuthService(UserManager<ApplicationUser> userManager, IOptions<JwtConfig> jwtConfig)
        {
            _userManager = userManager;
            _jwtConfig = jwtConfig.Value;
        }
        public async Task<AuthModel> RegisterUserAsync(RegisterModel registerModel)
        {
            // first check if user email is already exists in database
            if (await _userManager.FindByEmailAsync(registerModel.Email) is not null)
                return new AuthModel { Message = "Email already exists" };
            // check if username is already exists in database
            if (await _userManager.FindByNameAsync(registerModel.UserName) is not null)
                return new AuthModel { Message = "Username already exists" };
            // claim type exists in allowed types
            if (!CustomClaimTypes.ALLOWEDTYPES.Contains(registerModel.UserTypeClaim))
                return new AuthModel { Message = "Invalid claim type not allowed" };

            // create new user object
            var user = new ApplicationUser();
            user.Email = registerModel.Email;
            user.UserName = registerModel.UserName;
            user.Deposit = 0;

            // try to create user using password in db
            var result = await _userManager.CreateAsync(user, registerModel.Password);

            // if creating user failed return auth model with message of what went wrong
            if (!result.Succeeded)
            {
                string errorMessage = string.Empty;
                foreach (var error in result.Errors)
                {
                    errorMessage += $"{error.Description} | ";
                }
                return new AuthModel { Message = errorMessage };
            }
            // try to add the new claim to user (new claim for now (name and value == seller or buyyer))
            var tryAddClaimtoUser = await _userManager.AddClaimAsync(user, new Claim(registerModel.UserTypeClaim, registerModel.UserTypeClaim));
            //Todo check 
            // user creation went ok then create token and send it back
            var jwtToken = await CreateJwtTokenAsync(user);

            return new AuthModel
            {
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                UserTypeClaim = registerModel.UserTypeClaim,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                ExpiresOn = jwtToken.ValidTo
            };
        }
        public async Task<AuthModel> LoginUserAsync(LoginModel loginModel)
        {
            AuthModel authModel = new AuthModel();
            // return if email doesn't exist OR email+password don't match
            var user = await _userManager.FindByEmailAsync(loginModel.Email);
            if (user is null || !await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }

            var jwtToken = await CreateJwtTokenAsync(user);
            var claim = await _userManager.GetClaimsAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Username = user.UserName;
            authModel.Email = user.Email;
            authModel.UserTypeClaim = claim.First().Type;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.ExpiresOn = jwtToken.ValidTo;

            return authModel;
        }
        private async Task<JwtSecurityToken> CreateJwtTokenAsync(ApplicationUser user)
        {
            if (user is null) return null;
            // get user claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // create jwt claims
            var jwtClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            };
            // merge both claims lists and jwtClaims to allClaims
            var allClaims = jwtClaims.Union(userClaims);

            // specify the signing key and algorithm
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            // finally create the token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: allClaims,
                expires: DateTime.Now.AddHours(_jwtConfig.DurationInHours),
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;
        }
    }
}