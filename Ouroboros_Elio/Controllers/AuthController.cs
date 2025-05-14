using BusinessLogicLayer.Dtos.AuthDtos;
using BusinessLogicLayer.Dtos.MailUtils;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Ouroboros_Elio.Models;
using System.Security.Claims;

namespace Ouroboros_Elio.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IEmailService _emailService;
		private readonly IAuthService _authService;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEmailService emailService, IAuthService authService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_emailService = emailService;
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Register()
		{
			var model = new LoginRegisterViewModel
			{
				Register = new RegisterRequestViewModel(),
				Login = new LoginRequestViewModel()
			};
			return View(model);
		}

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login(LoginRegisterViewModel model)
		{
			var loginRequest = model.Login;

			if (!ModelState.IsValid)
			{
				return View("Register", model);
			}

			var user = await _userManager.FindByEmailAsync(loginRequest.Gmail);
			if (user == null)
			{
				ModelState.AddModelError("Login.Gmail", "Email does not exist.");
				return View("Register", model);
			}

			if (!user.EmailConfirmed)
			{
				ModelState.AddModelError("Login.Gmail", "Please verify your email before logging in.");
				return View("Register", model);
			}

			var result = await _signInManager.PasswordSignInAsync(user.UserName, loginRequest.Password, loginRequest.RememberMe, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				// Tùy chọn: ghi thông tin phụ vào cookie (không nên ghi nhạy cảm)
				Response.Cookies.Append("UserName", user.UserName, new CookieOptions { HttpOnly = true, Secure = true });

				return RedirectToAction("ProductList", "Product"); // Hoặc chuyển đến Dashboard
			}
			else if (result.IsLockedOut)
			{
				ModelState.AddModelError("", "Account is locked due to multiple failed login attempts.");
			}
			else
			{
				ModelState.AddModelError("Login.Password", "Incorrect password.");
			}
			TempData["Error"] = "Login failed. Please check your email or password.";
			return View("Register", model);
		}

		[HttpPost]
		[Route("RegisterAndSendMail")]
		public async Task<IActionResult> RegisterAndSendMail(LoginRegisterViewModel model)
		{
			var registerRequestViewModel = model.Register;
			if (!ModelState.IsValid)
			{
				return View("Register", model);
			}
			var user = await _userManager.FindByEmailAsync(registerRequestViewModel.Gmail);
			if (user != null)
			{
				ModelState.AddModelError("Gmail", "Email already exists.");
				return View("Register", model);
			}
			if (registerRequestViewModel.Password != registerRequestViewModel.ConfirmPassword)
			{
				ModelState.AddModelError("Password", "The password is not match.");
				return View("Register", model);
			}

			// Kiểm tra nếu số điện thoại đã tồn tại
			user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerRequestViewModel.Phone);
			if (user != null)
			{
				ModelState.AddModelError("Phone", "Phone number already exists.");
				return View("Register", model);
			}

			var identityUser = new ApplicationUser
			{
				UserName = registerRequestViewModel.UserName,
				Email = registerRequestViewModel.Gmail,
				PhoneNumber = registerRequestViewModel.Phone,
				Address = registerRequestViewModel.Address,
				RegistrationDate = DateTime.UtcNow,
				EmailConfirmed = false,
				VerificationToken = Guid.NewGuid().ToString(), // Tạo token xác thực email
			};

			var identityResult = await _userManager.CreateAsync(identityUser, registerRequestViewModel.Password);

			if (!identityResult.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "User register failed");
				return View("Register", model);
			}

			// Thêm roles cho người dùng
			string[] roles = new string[] { "User" };
			identityResult = await _userManager.AddToRolesAsync(identityUser, roles);

			if (!identityResult.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Role assignment failed.");
				return View("Register", model);
			}

			var verificationLink = Url.Action("VerifyEmail", "Auth", new { token = identityUser.VerificationToken }, Request.Scheme);

			// Gửi email xác thực
			var mailContent = new MailContent
			{
				To = identityUser.Email,
				Subject = "Email Verification",
				Body =
					$"<p>Click the link below to verify your email:</p><a href='{verificationLink}'>Verify Email</a>",
			};

			try
			{
				await _emailService.SendMail(mailContent);
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, $"Failed to send email: {ex.Message}");
				return View("Register", model);
			}

			TempData["SuccessMessage"] = "Registration successful. Please check your email to verify your account.";
			return RedirectToAction("Register");
		}

		[HttpGet]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			var result = await _authService.VerifyEmailAsync(token);
			if (!result)
			{
				ViewBag.Message = "Invalid or expired verification token.";
				return View("VerifyResult");
			}

			ViewBag.Message = "Email verified successfully.";
			return View("VerifyResult");
		}

		[HttpGet]
		public IActionResult VerifyResult()
		{
			return View();
		}
	}
}
