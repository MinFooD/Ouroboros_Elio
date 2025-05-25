using AutoMapper;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
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

		public DesignService(IDesignRepository designRepository, IMapper mapper)
		{
			_designRepository = designRepository;
			_mapper = mapper;
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

        public async Task<(List<DesignViewModel> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, int page, int pageSize)
        {
            var (designs, totalCount) = await _designRepository.GetPagedDesignsAsync(modelId, page, pageSize);
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

            return (designViewModels, totalCount);
        }

        public async Task<(List<DesignViewModel> Designs, int TotalCount)> GetPagedDesignsAsync(Guid? modelId, decimal? minPrice, decimal? maxPrice, int page, int pageSize)
        {
            var (designs, totalCount) = await _designRepository.GetPagedDesignsAsync(modelId, minPrice, maxPrice, page, pageSize);
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

            return (designViewModels, totalCount);
        }
    }
}
