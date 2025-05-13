using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
	public class ApplicationUser : IdentityUser<Guid>
	{
		public string? ProfileImage { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Address { get; set; }
		public DateTime RegistrationDate { get; set; }
		public string Status { get; set; } = "Active";
		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }
		public string? VerificationToken { get; set; } // Token xác thực email
		public string? ResetPasswordToken { get; set; }
		public DateTime? ResetPasswordExpiry { get; set; }
		public Cart Cart { get; set; }
		public ICollection<Order> Orders { get; set; }
		public ICollection<CustomBracelet> CustomBracelets { get; set; }
	}
}
