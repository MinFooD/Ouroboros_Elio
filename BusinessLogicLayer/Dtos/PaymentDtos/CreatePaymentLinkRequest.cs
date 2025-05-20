using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Dtos.PaymentDtos
{
	public record CreatePaymentLinkRequest(
		string productName,
		Guid cartId,
		string description,
		int price,
		string returnUrl,
		string cancelUrl
	);
}
