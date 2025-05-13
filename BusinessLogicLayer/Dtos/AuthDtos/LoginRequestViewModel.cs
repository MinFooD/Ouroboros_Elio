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
		[Required]
		[DataType(DataType.PhoneNumber)]
		[Phone]
		public string PhoneNumber { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
	}
}
