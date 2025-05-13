using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ServiceContracts
{
	public interface IAuthService
	{
		//string CreateJWTToken(ApplicationUser user, List<string> roles);
		Task<bool> VerifyEmailAsync(string token);
	}
}
