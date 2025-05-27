using BusinessLogicLayer.Dtos.CategoryDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Ouroboros_Elio.Models;
using System.Diagnostics;

namespace Ouroboros_Elio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDesignService _designService;

        public HomeController(ICategoryService categoryService, IDesignService designService)
        {
            _categoryService = categoryService;
            _designService = designService;
        }

        public async Task<IActionResult> Index()
        {
            // Get list of categories
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            // Get designs by category
            var designsByCategory = new Dictionary<Guid, List<DesignViewModel>>();
            if (categories != null)
            {
                foreach (var category in categories)
                {
                    var designs = await _designService.GetDesignsByCategoryAsync(category.CategoryId);
                    designsByCategory[category.CategoryId] = designs ?? new List<DesignViewModel>();
                }
            }
            ViewBag.DesignsByCategory = designsByCategory;

            // Get top ordered designs
            var topOrderedDesigns = await _designService.GetTopOrderedDesignsAsync(6); // Limit to 6 products
            ViewBag.TopOrderedDesigns = topOrderedDesigns ?? new List<DesignViewModel>();

            return View();
        }

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
