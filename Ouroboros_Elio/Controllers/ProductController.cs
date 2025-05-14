using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ouroboros_Elio.Controllers
{
    public class ProductController : Controller
    {
		private readonly ILogger<ProductController> _logger;
		private readonly IDesignService _designService;

		public ProductController(ILogger<ProductController> logger, IDesignService designService)
		{
			_logger = logger;
			_designService = designService;
		}

		[Authorize(Roles = "Admin,Manager,User")]
		public async Task<IActionResult> ProductList()
        {
            var designs = await _designService.GetAllDesignsAsync();
			return View(designs);
        }

        public IActionResult ProductDetail(Guid designId)
        {
			var design = _designService.GetDesignByIdAsync(designId);
			if (design == null)
			{
				return NotFound();
			}
			return View(design);
		}
    }
}
