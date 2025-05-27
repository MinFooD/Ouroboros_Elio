using BusinessLogicLayer.Dtos.CategoryDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.ModelDtos;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Ouroboros_Elio.Models;
using System.Diagnostics;

namespace Ouroboros_Elio.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IDesignService _designService;
        private readonly IModelService _modelService;

        public HomeController(ICategoryService categoryService, IDesignService designService, IModelService modelService)
        {
            _categoryService = categoryService;
            _designService = designService;
            _modelService = modelService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new HomeIndexViewModel();
            model.Models = await _modelService.GetAllActiveModelsAsync() ?? new List<ModelViewModel>();
            model.Categories = await _categoryService.GetAllCategoriesAsync() ?? new List<CategoryViewModel>();
            foreach (var category in model.Categories)
            {
                model.DesignsByCategory[category.CategoryId] = await _designService.GetDesignsByCategoryAsync(category.CategoryId) ?? new List<DesignViewModel>();
            }
            model.TopOrderedDesigns = await _designService.GetTopOrderedDesignsAsync(6) ?? new List<DesignViewModel>();
            return View(model);
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
