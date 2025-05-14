using AutoMapper;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.ServiceContracts;
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

		public async Task<List<DesignViewModel>?> GetAllDesignsAsync()
		{
			var designs = await _designRepository.GetAllDesignsAsync();
			if (designs == null || designs.Count == 0)
			{
				return null;
			}

			var designViewModels = _mapper.Map<List<DesignViewModel>>(designs);

			for (int i = 0; i < designs.Count; i++)
			{
				var modelName = designs[i].Model.ModelName;
				var topicName = designs[i].Model.Topic.TopicName; 
				var collectionName = designs[i].Model.Topic.Collection.CollectionName;
				designViewModels[i].DesignName = $"{collectionName}-{topicName}-{modelName}";
				designViewModels[i].FirstImage = _mapper.Map<DesignImageViewModel>(designs[i].DesignImages.FirstOrDefault());
			}
			return designViewModels;
		}

		public async Task<DesignViewModel?> GetDesignByIdAsync(Guid designId)
		{
			var design = await _designRepository.GetDesignByIdAsync(designId);
			if (design == null)
			{
				return null;
			}
			var modelName = design.Model.ModelName;
			var topicName = design.Model.Topic.TopicName;
			var collectionName = design.Model.Topic.Collection.CollectionName;
			var designViewModel = _mapper.Map<DesignViewModel>(design);
			designViewModel.FirstImage = _mapper.Map<DesignImageViewModel>(design.DesignImages.FirstOrDefault());
			designViewModel.DesignName = $"{collectionName}-{topicName}-{modelName}";
			return designViewModel;
		}
	}
}
