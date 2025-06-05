using AutoMapper;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class DesignService : IDesignService
    {
        private readonly IDesignRepository _designRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string TopOrderedDesignsCacheKey = "TopOrderedDesigns";
        private const string PagedDesignsCacheKey = "PagedDesigns";
        private readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

        public DesignService(IDesignRepository designRepository, IMapper mapper, IMemoryCache cache)
        {
            _designRepository = designRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<DesignViewModel>?> GetAllDesignsAsync(Guid? modelId)
        {
            var designs = await _designRepository.GetAllDesignsAsync(modelId);
            if (designs == null || designs.Count == 0)
            {
                return null;
            }

            var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

            for (int i = 0; i < designs.Count; i++)
            {
                var categoryName = designs[i].Category.CategoryName;
                var modelName = designs[i].Model.ModelName;
                var topicName = designs[i].Model.Topic.TopicName;
                var collectionName = designs[i].Model.Topic.Collection.CollectionName;
                designViewModels[i].DesignName = $"{collectionName}-{topicName}-{modelName}-{categoryName}";
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(designs[i].DesignImages.FirstOrDefault());
                designViewModels[i].CollectionName = collectionName;
                designViewModels[i].ModelName = modelName;
                designViewModels[i].TopicName = topicName;
                designViewModels[i].CategoryName = categoryName;
            }
            return designViewModels;
        }

        public async Task<DesignViewModel?> GetDesignByIdAsync(Guid? designId)
        {
            var design = await _designRepository.GetDesignByIdAsync(designId);
            if (design == null)
            {
                return null;
            }
            var categoryName = design.Category.CategoryName;
            var modelName = design.Model.ModelName;
            var topicName = design.Model.Topic.TopicName;
            var collectionName = design.Model.Topic.Collection.CollectionName;
            var designViewModel = _mapper.Map<DesignViewModel>(design);
            designViewModel.FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            designViewModel.DesignName = $"{collectionName} - {topicName} - {modelName}";
            designViewModel.CollectionName = collectionName;
            designViewModel.ModelName = modelName;
            designViewModel.TopicName = topicName;
            designViewModel.CategoryName = categoryName;
            return designViewModel;
        }

        public async Task<bool> VisitCountUp(Guid designId)
        {
            return await _designRepository.VisitCountUp(designId);
        }

        public async Task<(List<DesignViewModel> Designs, int TotalCount)>
            GetPagedDesignsAsync
            (
            Guid? modelId,
            int page,
            int pageSize,
            string? sortBy = null,
            string? searchQuery = null
            )
        {
            var cacheKey = $"{PagedDesignsCacheKey}" +
                $"_{modelId}" +
                $"_{page}" +
                $"_{pageSize}" +
                $"_{sortBy}" +
                $"_{searchQuery}";

            if (_cache.TryGetValue(cacheKey, out (List<DesignViewModel> Designs, int TotalCount) cachedResult))
            {
                return cachedResult;
            }

            var (designs, totalCount) = await _designRepository
                .GetPagedDesignsAsync
                (modelId, page, pageSize, sortBy, searchQuery);
            var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

            for (int i = 0; i < designs.Count; i++)
            {
                var design = designs[i];
                designViewModels[i].DesignName =
                    $"{design.Model.Topic.Collection.CollectionName}" +
                    $"-{design.Model.Topic.TopicName}" +
                    $"-{design.Model.ModelName}" +
                    $"-{design.Category.CategoryName}";
                designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
                designViewModels[i].ModelName = design.Model.ModelName;
                designViewModels[i].TopicName = design.Model.Topic.TopicName;
                designViewModels[i].CategoryName = design.Category.CategoryName;
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            }

            var result = (designViewModels, totalCount);
            _cache.Set(cacheKey, result, CacheDuration);

            return result;
        }

        public async Task<(List<DesignViewModel> Designs, int TotalCount)>
            GetPagedDesignsAsync
            (
            Guid? modelId,
            decimal? minPrice,
            decimal? maxPrice,
            int page,
            int pageSize,
            string? sortBy = null,
            string? searchQuery = null
            )
        {
            var cacheKey = $"{PagedDesignsCacheKey}" +
                $"_{modelId}_{minPrice}" +
                $"_{maxPrice}_{page}" +
                $"_{pageSize}_{sortBy}" +
                $"_{searchQuery}";
            if (_cache.TryGetValue(cacheKey, out (List<DesignViewModel> Designs, int TotalCount) cachedResult))
            {
                return cachedResult;
            }

            var (designs, totalCount) = await _designRepository
                .GetPagedDesignsAsync
                (modelId, minPrice, maxPrice, page, pageSize, sortBy, searchQuery);
            var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

            for (int i = 0; i < designs.Count; i++)
            {
                var design = designs[i];
                designViewModels[i].DesignName =
                    $"{design.Model.Topic.Collection.CollectionName}" +
                    $"-{design.Model.Topic.TopicName}" +
                    $"-{design.Model.ModelName}" +
                    $"-{design.Category.CategoryName}";
                designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
                designViewModels[i].ModelName = design.Model.ModelName;
                designViewModels[i].TopicName = design.Model.Topic.TopicName;
                designViewModels[i].CategoryName = design.Category.CategoryName;
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            }

            var result = (designViewModels, totalCount);
            _cache.Set(cacheKey, result, CacheDuration);

            return result;
        }

        public async Task<List<DesignViewModel>?> GetDesignsByCategoryAsync(Guid categoryId)
        {
            var designs = await _designRepository.GetAllDesignsAsync(null);
            if (designs == null)
            {
                return null;
            }

            var filteredDesigns = designs.Where(d => d.CategoryId == categoryId).ToList();
            var designViewModels = _mapper.Map<List<DesignViewModel>>(filteredDesigns);

            for (int i = 0; i < filteredDesigns.Count; i++)
            {
                var design = filteredDesigns[i];
                designViewModels[i].DesignName = $"{design.Model.Topic.Collection.CollectionName}-{design.Model.Topic.TopicName}-{design.Model.ModelName}-{design.Category.CategoryName}";
                designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
                designViewModels[i].ModelName = design.Model.ModelName;
                designViewModels[i].TopicName = design.Model.Topic.TopicName;
                designViewModels[i].CategoryName = design.Category.CategoryName;
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            }

            return designViewModels;
        }

        public async Task<List<DesignViewModel>> GetTopOrderedDesignsAsync(int topCount)
        {
            // Try to get from cache
            if (_cache.TryGetValue($"{TopOrderedDesignsCacheKey}_{topCount}", out List<DesignViewModel> cachedDesigns))
            {
                return cachedDesigns;
            }

            // Fetch from database
            var designs = await _designRepository.GetTopOrderedDesignsAsync(topCount);
            var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

            for (int i = 0; i < designs.Count; i++)
            {
                var design = designs[i];
                designViewModels[i].DesignName = $"{design.Model.Topic.Collection.CollectionName}-{design.Model.Topic.TopicName}-{design.Model.ModelName}-{design.Category.CategoryName}";
                designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
                designViewModels[i].ModelName = design.Model.ModelName;
                designViewModels[i].TopicName = design.Model.Topic.TopicName;
                designViewModels[i].CategoryName = design.Category.CategoryName;
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            }

            // Store in cache
            _cache.Set($"{TopOrderedDesignsCacheKey}_{topCount}", designViewModels, CacheDuration);

            return designViewModels;
        }

        public async Task<List<DesignViewModel>> GetDesignsByIdsAsync(List<Guid?> designIds)
        {
            var designs = await _designRepository.GetDesignsByIdsAsync(designIds);
            var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

            for (int i = 0; i < designs.Count; i++)
            {
                var design = designs[i];
                designViewModels[i].DesignName =
                    $"{design.Model.Topic.Collection.CollectionName}" +
                    $"-{design.Model.Topic.TopicName}" +
                    $"-{design.Model.ModelName}" +
                    $"-{design.Category.CategoryName}";
                designViewModels[i].CollectionName = design.Model.Topic.Collection.CollectionName;
                designViewModels[i].ModelName = design.Model.ModelName;
                designViewModels[i].TopicName = design.Model.Topic.TopicName;
                designViewModels[i].CategoryName = design.Category.CategoryName;
                designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
            }

            return designViewModels;
        }
    }
}
