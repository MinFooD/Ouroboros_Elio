using AutoMapper;
using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.Dtos.DesignDtos;
using BusinessLogicLayer.Dtos.ModelDtos;
using BusinessLogicLayer.Dtos.OrderDtos;
using BusinessLogicLayer.Dtos.PaymentDtos;
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
			//CreateMap<Design, DesignViewModel>()
			//    .ForMember(dest => dest.DesignImages, opt => opt.MapFrom(src => src.DesignImages))
			//    .ForMember(dest => dest.DesignName, opt => opt.MapFrom(src => $"{src.Model.Topic.Collection.CollectionName}-{src.Model.Topic.TopicName}-{src.Model.ModelName}-{src.Category.CategoryName}"))
			//    .ForMember(dest => dest.CollectionName, opt => opt.MapFrom(src => src.Model.Topic.Collection.CollectionName))
			//    .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Model.Topic.TopicName))
			//    .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.Model.ModelName))
			//    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
			//    .ForMember(dest => dest.FirstImage, opt => opt.MapFrom(src => src.DesignImages.FirstOrDefault()))
			//    .ReverseMap();

			CreateMap<Model, ModelViewModel>()
				.ReverseMap();
			CreateMap<CartItem,CartItemViewModel>()
				.ReverseMap();
			CreateMap<Cart, CartViewModel>()
				.ReverseMap();
			CreateMap<Order, OrderViewModel>()
				.ReverseMap();
			CreateMap<OrderItem, OrderItemsViewModel>()
				.ReverseMap();
			CreateMap<Payment, PaymentViewModel>()
				.ReverseMap();
		}
	}
}
