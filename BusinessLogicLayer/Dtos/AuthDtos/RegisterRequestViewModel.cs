using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.AuthDtos
{
	public class RegisterRequestViewModel
	{
		[DataType(DataType.EmailAddress)]
		public string Gmail { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[StringLength(50)]
		public string UserName { get; set; }

		[StringLength(10)]
		public string Phone { get; set; }

		[StringLength(255)]
		public string Address { get; set; }
	}
}
