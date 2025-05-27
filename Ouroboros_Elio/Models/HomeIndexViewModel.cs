using BusinessLogicLayer.Dtos.CategoryDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.ModelDtos;

namespace Ouroboros_Elio.Models;

public class HomeIndexViewModel
{
    public List<ModelViewModel> Models { get; set; } = new List<ModelViewModel>();
    public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    public Dictionary<Guid, List<DesignViewModel>> DesignsByCategory { get; set; } = new Dictionary<Guid, List<DesignViewModel>>();
    public List<DesignViewModel> TopOrderedDesigns { get; set; } = new List<DesignViewModel>();
}