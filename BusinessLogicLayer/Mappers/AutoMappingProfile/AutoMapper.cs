using AutoMapper;
using BusinessLogicLayer.Dtos.CartDtos;
using BusinessLogicLayer.Dtos.CategoryDtos;
using BusinessLogicLayer.Dtos.CharmDtos;
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

			CreateMap<Model, ModelViewModel>()
				.ReverseMap();

			CreateMap<CartItem,CartItemViewModel>()
                //.ReverseMap();
				.ForMember(dest => dest.Design, opt => opt.Ignore());
            CreateMap<Cart, CartViewModel>()
				.ReverseMap();

			CreateMap<Order, OrderViewModel>()
				.ReverseMap();
			CreateMap<OrderItem, OrderItemsViewModel>()
				.ReverseMap();

			CreateMap<Payment, PaymentViewModel>()
				.ReverseMap();

            CreateMap<Category, CategoryViewModel>().ReverseMap();
			CreateMap<Charm, CharmViewModel>().ReverseMap();
			CreateMap<CustomBraceletCharm, CustomBraceletCharmViewModel>().ReverseMap();
			CreateMap<CustomBracelet, CustomBraceletViewModel>().ReverseMap();
		}
	}
}
