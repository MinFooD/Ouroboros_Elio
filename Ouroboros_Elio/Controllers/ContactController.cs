using BusinessLogicLayer.Dtos.ContactDtos;
using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ouroboros_Elio.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _recaptchaSecretKey;
        private readonly ILogger _logger;

        public ContactController(IContactService contactService, IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<ContactController> logger)
        {
            _contactService = contactService;
            _httpClientFactory = httpClientFactory;
            _recaptchaSecretKey = configuration["Recaptcha:SecretKey"];
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(ContactMessageCreateDto contactMessageDto, [FromForm(Name = "g-recaptcha-response")] string gRecaptchaResponse)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("ModelState không hợp lệ. Lỗi: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
            }

            // Kiểm tra gRecaptchaResponse
            if (string.IsNullOrEmpty(gRecaptchaResponse))
            {
                _logger.LogWarning("Token reCAPTCHA trống hoặc không được gửi.");
                return Json(new { success = false, message = "Vui lòng hoàn thành xác thực reCAPTCHA." });
            }

            // Xác thực reCAPTCHA
            var isRecaptchaValid = await VerifyRecaptchaAsync(gRecaptchaResponse);
            if (!isRecaptchaValid)
            {
                _logger.LogWarning("Xác thực reCAPTCHA không thành công.");
                return Json(new { success = false, message = "Xác thực reCAPTCHA không thành công. Vui lòng thử lại." });
            }

            try
            {
                var contactMessage = await _contactService.SendContactMessageAsync(contactMessageDto);
                return Json(new { success = true, message = "Tin nhắn đã được gửi thành công!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gửi tin nhắn liên hệ.");
                return Json(new { success = false, message = "Đã có lỗi xảy ra. Vui lòng thử lại sau." });
            }
        }

        private async Task<bool> VerifyRecaptchaAsync(string recaptchaResponse)
        {
            var client = _httpClientFactory.CreateClient();
            var requestUrl = "https://www.google.com/recaptcha/api/siteverify";
            var content = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("secret", _recaptchaSecretKey),
            new KeyValuePair<string, string>("response", recaptchaResponse),
            new KeyValuePair<string, string>("remoteip", HttpContext.Connection.RemoteIpAddress?.ToString())
        });

            try
            {
                var response = await client.PostAsync(requestUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Yêu cầu xác thực reCAPTCHA thất bại với mã trạng thái: {StatusCode}", response.StatusCode);
                    return false;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("reCAPTCHA JSON Response: {JsonResponse}", jsonResponse);

                // Sử dụng tùy chọn không phân biệt hoa thường khi deserialize
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var recaptchaResult = JsonSerializer.Deserialize<RecaptchaResponse>(jsonResponse, options);
                _logger.LogInformation("reCAPTCHA Result: Success={Success}, ErrorCodes={ErrorCodes}, ChallengeTs={ChallengeTs}, Hostname={Hostname}",
                recaptchaResult.Success,
                recaptchaResult.ErrorCodes != null ? string.Join(", ", recaptchaResult.ErrorCodes) : "null",
                recaptchaResult.ChallengeTs,
                recaptchaResult.Hostname);

                if (!recaptchaResult.Success)
                {
                    _logger.LogWarning("Xác thực reCAPTCHA thất bại. Mã lỗi: {ErrorCodes}", string.Join(", ", recaptchaResult.ErrorCodes ?? Array.Empty<string>()));
                }

                return recaptchaResult.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực reCAPTCHA.");
                return false;
            }
        }
    }

    public class RecaptchaResponse
    {
        public bool Success { get; set; }
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }
        [JsonPropertyName("challenge_ts")]
        public string ChallengeTs { get; set; }
        public string Hostname { get; set; }
    }
}

