using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
	public class AuthService : IAuthService
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		public string CreateJWTToken(ApplicationUser user, List<string> roles)
		{
			var claims = new List<Claim>();

			claims.Add(new Claim(ClaimTypes.Email, user.Email));
			claims.Add(new Claim(ClaimTypes.Name, user.UserName));
			claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: DateTime.Now.AddMinutes(15),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		//public async Task<bool> PasswordSignInAsync(string gmail, string password, bool rememberMe, bool lockoutOnFailure)
		//{
		//	var user = await _userManager.FindByEmailAsync(gmail);

		//	if (user == null)
		//	{
		//		return false;
		//	}

		//	if (!user.EmailConfirmed)
		//	{
		//		return false;
		//	}

		//	var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure);

		//	return result.Succeeded;
		//}

		public async Task<bool> VerifyEmailAsync(string token)
		{
			var user = await _userManager.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
			if (user == null)
			{
				return false; // Token không hợp lệ
			}
			user.EmailConfirmed = true; // Đánh dấu email đã xác thực
			user.VerificationToken = null; // Xóa token xác thực
			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded;
		}
	}
}
