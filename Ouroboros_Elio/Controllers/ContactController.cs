using BusinessLogicLayer.Dtos.ContactDtos;
using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(ContactMessageCreateDto contactMessageDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Vui lòng điền đầy đủ các trường bắt buộc." });
            }

            var (success, message) = await _contactService.SendContactMessageAsync(contactMessageDto);
            return Json(new { success, message });
        }
    }
}
