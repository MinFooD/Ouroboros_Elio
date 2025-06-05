using AutoMapper;
using BusinessLogicLayer.Dtos.CharmDtos;
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
	public class CharmService : ICharmService
	{
		private readonly ICharmRepository _charmRepository;
		private readonly ICustomBraceletRepository _customBraceletRepository;
		private readonly IMapper _mapper;
		private readonly ICartRepository _cartRepository;

		public CharmService(ICharmRepository charmRepository, ICustomBraceletRepository customBraceletRepository, IMapper mapper, ICartRepository cartRepository)
		{
			_charmRepository = charmRepository;
			_customBraceletRepository = customBraceletRepository;
			_mapper = mapper;
			_cartRepository = cartRepository;
		}

		public async Task<(bool? Success, string Message)> CreateCustomBracelet(string braceletName,string note, List<string> charms, Guid userId)
		{
			var result = false;
			var oldBracelet = await _charmRepository.GetCustomBraceletByUserIdAsync(userId);
			if (oldBracelet != null)
			{
                var updateResult = await _cartRepository.UpdateQuantity(userId, oldBracelet.CustomBraceletId, 0, true);
				if(updateResult.Success == false)
				{
					return (false, ""); // Không thể xóa vòng tay cũ
				}
			}
			result = await _charmRepository.CreateCustomBracelet(braceletName, note, charms, userId);
			return (result, "");
		}

		public async Task<List<CharmViewModel>> GetAllCharmsAsync()
		{
			var charms = await _charmRepository.GetAllCharmsAsync();
			var charmViewModels = _mapper.Map<List<CharmViewModel>>(charms);
			return charmViewModels;
		}

		public async Task<CharmViewModel[]> GetArrayCharmsAsync()
		{
			var charms = await _charmRepository.GetAllCharmsAsync();
			var charmViewModels = _mapper.Map<List<CharmViewModel>>(charms);
			return charmViewModels.ToArray();
		}

		public async Task<CharmViewModel?> GetCharmByIdAsync(Guid charmId)
		{
			var charm = await _charmRepository.GetCharmByIdAsync(charmId);
			var charmViewModel = charm != null ? _mapper.Map<CharmViewModel>(charm) : null;
			return charmViewModel;
		}

		public async Task<CustomBraceletViewModel?> GetCustomBraceletByIdAsync(Guid? customBraceletId)
		{
			var customBracelet = await _customBraceletRepository.GetCustomBraceletByIdAsync(customBraceletId);
			var customBraceletViewModel = customBracelet != null ? _mapper.Map<CustomBraceletViewModel>(customBracelet) : null;
			return customBraceletViewModel;
		}

		public async Task<List<CustomBraceletCharmViewModel>> GetCustomBraceletCharm(Guid? customBraceletId)
		{
			var customBraceletCharms = await _charmRepository.GetCustomBraceletCharm(customBraceletId);
			var customBraceletCharmViewModel = _mapper.Map<List<CustomBraceletCharmViewModel>>(customBraceletCharms);
			return customBraceletCharmViewModel;
		}
	}
}
