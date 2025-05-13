using BusinessLogicLayer.Dtos.AuthDtos;
using BusinessLogicLayer.Dtos.MailUtils;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Ouroboros_Elio.Controllers
{
	public class AuthController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IEmailService _emailService;
		private readonly IAuthService _authService;

		public AuthController(UserManager<ApplicationUser> userManager, IEmailService emailService, IAuthService authService)
		{
			_userManager = userManager;
			_emailService = emailService;
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		[Route("RegisterAndSendMail")]
		public async Task<IActionResult> RegisterAndSendMail(RegisterRequestViewModel registerRequestViewModel)
		{
			if (!ModelState.IsValid)
			{
				return View("Register", registerRequestViewModel);
			}
			var user = await _userManager.FindByEmailAsync(registerRequestViewModel.Gmail);
			if (user != null)
			{
				ModelState.AddModelError("Gmail", "Email already exists.");
				return View("Register", registerRequestViewModel);
			}

			// Kiểm tra nếu số điện thoại đã tồn tại
			user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == registerRequestViewModel.Phone);
			if (user != null)
			{
				ModelState.AddModelError("Phone", "Phone number already exists.");
				return View("Register", registerRequestViewModel);
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
				return View("Register", registerRequestViewModel);
			}

			// Thêm roles cho người dùng
			string[] roles = new string[] { "User" };
			identityResult = await _userManager.AddToRolesAsync(identityUser, roles);

			if (!identityResult.Succeeded)
			{
				ModelState.AddModelError(string.Empty, "Role assignment failed.");
				return View("Register", registerRequestViewModel);
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
				return View("Register", registerRequestViewModel);
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
