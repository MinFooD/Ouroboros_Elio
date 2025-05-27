using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace Ouroboros_Elio.Controllers;

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

    public async Task<IActionResult> 
		ProductList
		(
		Guid? modelId, 
		decimal? minPrice, 
		decimal? maxPrice, 
		int page = 1, 
		int pageSize = 6,
        string? sortBy = null,
        string? searchQuery = null
        )
    {
        var (designs, totalCount) = minPrice.HasValue || maxPrice.HasValue
            ? await _designService
			.GetPagedDesignsAsync
			(modelId, minPrice, maxPrice, page, pageSize, sortBy, searchQuery)
            : await _designService
			.GetPagedDesignsAsync
			(modelId, page, pageSize, sortBy, searchQuery);

        ViewBag.ListModel = await _modelService.GetAllActiveModelsAsync();
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = totalCount;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        ViewBag.ModelId = modelId;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;
        ViewBag.SortBy = sortBy;
        ViewBag.SearchQuery = searchQuery;

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
