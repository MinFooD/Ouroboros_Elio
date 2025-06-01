using AutoMapper;
using BusinessLogicLayer.Dtos.ModelDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Caching.Memory;

namespace BusinessLogicLayer.Services;

public class ModelService : IModelService
{
    private readonly IModelRepository _modelRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string ActiveModelsCacheKey = "ActiveModels";
    private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    public ModelService(IModelRepository modelRepository, IMapper mapper, IMemoryCache cache)
    {
        _modelRepository = modelRepository;
        _mapper = mapper;
        _cache = cache;
    }
    public async Task<List<ModelViewModel>?> GetAllActiveModelsAsync()
    {
        // Try to get from cache
        if (_cache.TryGetValue(ActiveModelsCacheKey, out List<ModelViewModel> cachedModels))
        {
            return cachedModels;
        }

        // Fetch from database
        var models = await _modelRepository.GetAllActiveModelsAsync();
        if (models == null)
        {
            return null;
        }

        var modelViewModels = _mapper.Map<List<ModelViewModel>>(models);

        // Store in cache
        _cache.Set(ActiveModelsCacheKey, modelViewModels, CacheDuration);

        return modelViewModels;
    }
}
