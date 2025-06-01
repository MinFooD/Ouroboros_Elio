using AutoMapper;
using BusinessLogicLayer.Dtos.ContactDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services;

public class ContactService : IContactService
{
    private readonly IContactRepository _contactRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public ContactService(IContactRepository contactRepository, IMapper mapper, IConfiguration configuration)
    {
        _contactRepository = contactRepository;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<bool> SendContactMessageAsync(ContactMessageCreateDto contactMessageDto)
    {
        try
        {
            // Lưu vào cơ sở dữ liệu
            var contactMessage = _mapper.Map<ContactMessage>(contactMessageDto);
            await _contactRepository.AddContactMessageAsync(contactMessage);

            // Gửi email thông báo
            await SendEmailAsync(contactMessageDto);

            return true;
        }
        catch (SmtpException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private async Task SendEmailAsync(ContactMessageCreateDto contactMessageDto)
    {
        var smtpClient = new SmtpClient(_configuration["SmtpSettings:Server"])
        {
            Port = int.Parse(_configuration["SmtpSettings:Port"]),
            Credentials = new NetworkCredential(_configuration["SmtpSettings:Username"], _configuration["SmtpSettings:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_configuration["SmtpSettings:SenderEmail"], _configuration["SmtpSettings:SenderName"]),
            Subject = $"Tin nhắn liên hệ mới: {contactMessageDto.Subject ?? "Không có chủ đề"}",
            Body = $"<h3>Tin nhắn từ {contactMessageDto.Name}</h3>" +
                   $"<p><strong>Email:</strong> {contactMessageDto.Email}</p>" +
                   $"<p><strong>Chủ đề:</strong> {contactMessageDto.Subject}</p>" +
                   $"<p><strong>Nội dung:</strong> {contactMessageDto.Message}</p>",
            IsBodyHtml = true,
        };

        mailMessage.ReplyToList.Add(new MailAddress(contactMessageDto.Email, contactMessageDto.Name));

        mailMessage.To.Add(_configuration["SmtpSettings:AdminEmail"]);

        await smtpClient.SendMailAsync(mailMessage);
    }
}