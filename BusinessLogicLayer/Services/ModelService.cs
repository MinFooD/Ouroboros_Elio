using AutoMapper;
using BusinessLogicLayer.Dtos.ModelDtos;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.RepositoryContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
	public class ModelService : IModelService
	{
		private readonly IModelRepository _modelRepository;
		private readonly IMapper _mapper;
		public ModelService(IModelRepository modelRepository, IMapper mapper)
		{
			_modelRepository = modelRepository;
			_mapper = mapper;
		}
		public async Task<List<ModelViewModel>?> GetAllModelsAsync()
		{
			var models = await _modelRepository.GetAllModelsAsync();
			if (models == null)
			{
				return null;
			}
			var modelViewModels = _mapper.Map<List<ModelViewModel>>(models);
			return modelViewModels;
		}
	}
}
