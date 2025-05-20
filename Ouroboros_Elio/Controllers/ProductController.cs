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
		private readonly IModelService _modelService;

		public ProductController(ILogger<ProductController> logger, IDesignService designService, IModelService modelService)
		{
			_logger = logger;
			_designService = designService;
			_modelService = modelService;
		}

		//[Authorize(Roles = "Admin,Manager,User")]
		public async Task<IActionResult> ProductList(Guid? modelId)
        {
            var designs = await _designService.GetAllDesignsAsync(modelId);
			ViewBag.ListModel = await _modelService.GetAllModelsAsync();
			return View(designs);
        }

        public async Task<IActionResult> ProductDetail(Guid designId)
        {
			var design = await _designService.GetDesignByIdAsync(designId);
			var result = await _designService.VisitCountUp(designId);
			if(result == false)   
			{
				return NotFound();
			}
			if (design == null)
			{
				return NotFound();
			}
			return View(design);
		}
    }
}
