﻿using BusinessLogicLayer.Dtos.MailUtils;
using BusinessLogicLayer.ServiceContracts;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
	public class EmailService : IEmailService
	{
		private readonly MailSettings _mailSettings;
		private readonly ILogger<EmailService> _logger;

		public EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger)
		{
			_mailSettings = mailSettings.Value;
			_logger = logger;
			_logger.LogInformation("SendMailService created");
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			await SendMail(
				new MailContent
				{
					To = email,
					Subject = subject,
					Body = htmlMessage,
				}
			);
		}

		public async Task SendMail(MailContent mailContent)
		{
			var email = new MimeMessage();
			email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
			email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
			email.To.Add(MailboxAddress.Parse(mailContent.To));
			email.Subject = mailContent.Subject;

			var builder = new BodyBuilder();
			builder.HtmlBody = mailContent.Body;
			email.Body = builder.ToMessageBody();

			using var smtp = new MailKit.Net.Smtp.SmtpClient();
			try
			{
				smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
				smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
				await smtp.SendAsync(email);
			}
			catch (Exception ex)
			{
				System.IO.Directory.CreateDirectory("mailssave");
				var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
				await email.WriteToAsync(emailsavefile);

				_logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
				_logger.LogError(ex.Message);
			}
			finally
			{
				smtp.Disconnect(true);
			}
		}
	}
}
