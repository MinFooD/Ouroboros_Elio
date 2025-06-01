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
                return Json(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var contactMessage = await _contactService.SendContactMessageAsync(contactMessageDto);
            return Json(new { success = true });
        }
    }
}
