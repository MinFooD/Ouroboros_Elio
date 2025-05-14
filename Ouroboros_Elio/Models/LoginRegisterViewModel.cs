using BusinessLogicLayer.Dtos.AuthDtos;

namespace Ouroboros_Elio.Models
{
	public class LoginRegisterViewModel
	{
		public RegisterRequestViewModel? Register { get; set; }
		public LoginRequestViewModel? Login { get; set; }
	}
}
