using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;
using BusinessLogicLayer.ServiceContracts;
using Twilio.Rest.Verify.V2.Service;

namespace BusinessLogicLayer.Services
{
	public class OtpVoiceService : IOtpVoiceService
	{
		private readonly string _accountSid;
		private readonly string _authToken;
		private readonly string _twilioPhoneNumber;

		public OtpVoiceService(IConfiguration configuration)
		{
			_accountSid = configuration["Twilio:AccountSID"];
			_authToken = configuration["Twilio:AuthToken"];
			_twilioPhoneNumber = configuration["Twilio:PhoneNumber"];
		}

		public async Task SendOtpViaVoice(string phoneNumber, string otp)
		{
			TwilioClient.Init(_accountSid, _authToken);

			//var call = CallResource.Create(
			//	to: new PhoneNumber(phoneNumber),
			//	from: new PhoneNumber(_twilioPhoneNumber),
			//	url: new Uri($"http://twimlets.com/message?Message[0]=Your OTP is: {otp}")
			//);

			var verification = VerificationResource.Create(
				to: $"+84{phoneNumber}",
				channel: "call",
				pathServiceSid: "VAf8a5b2a1a0073937dd1f8369939b6586"
			);
		}
	}
}
