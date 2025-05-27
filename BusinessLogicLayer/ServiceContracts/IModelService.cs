using BusinessLogicLayer.Dtos.ModelDtos;

namespace BusinessLogicLayer.ServiceContracts;

	public interface IModelService
	{
    Task<List<ModelViewModel>?> GetAllActiveModelsAsync();
}
