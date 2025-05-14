using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.AuthDtos
{
	public class LoginRequestViewModel
	{
		[DataType(DataType.EmailAddress)]
		public string Gmail { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public bool RememberMe { get; set; } = false;
	}
}
