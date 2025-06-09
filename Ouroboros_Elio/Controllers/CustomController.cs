using BusinessLogicLayer.Dtos.CharmDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ouroboros_Elio.Controllers
{
	public class CustomController : Controller
	{
		private readonly ICharmService _charmService;
		private readonly UserManager<ApplicationUser> _userManager;
		public CustomController(ICharmService charmService, UserManager<ApplicationUser> userManager)
		{
			_charmService = charmService;
			_userManager = userManager;
		}

		[HttpGet]
		public async Task<IActionResult> Custom()
		{
			var charms = await _charmService.GetArrayCharmsAsync();
			var charm = charms.Select(c => new
			{
				c.CharmId,
				c.Name,
				c.Price,
				// Thêm resize parameters
				ImageUrl = $"{c.ImageUrl}?width=100&height=100&mode=crop&quality=80"
			}).ToList();
			ViewBag.Charms = charms;
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddBraceletToCart([FromBody] BraceletRequest request)
		{
			var note = request.Note;
			var charmIds = request.CharmIds;
			var currentUser = HttpContext.User;
			var userId = _userManager.GetUserId(currentUser);
			if (userId == null)
			{
				return Json(new { success = false, message = "Vui lòng đăng nhập." });
			}

			var result = await _charmService.CreateCustomBracelet("Custom Bracelet", note, charmIds, Guid.Parse(userId));

			if(result.Success == true)
			{
				return Json(new { success = true, message = "Custom bracelet added to cart successfully." });
			}

			return Json(new { success = false, message = "Custom bracelet added to cart failed." });
		}
	}
}
