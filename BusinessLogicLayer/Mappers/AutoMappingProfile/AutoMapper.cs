using AutoMapper;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.ModelDtos;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Mappers.AutoMappingProfile
{
	public class AutoMapper : Profile
	{
		public AutoMapper()
		{
			CreateMap<Design, DesignViewModel>()
				.ForMember(dest => dest.DesignImages, opt => opt.MapFrom(src => src.DesignImages))
				.ReverseMap();
			CreateMap<DesignImage, DesignImageViewModel>()
				.ForMember(dest => dest.Design, opt => opt.MapFrom(src => src.Design))
				.ReverseMap();
			CreateMap<Model, ModelViewModel>()
				.ReverseMap();
		}
	}
}
