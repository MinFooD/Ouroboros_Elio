﻿using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddBusinessLogicLayer(this IServiceCollection services)
		{
			var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;
			services.AddAutoMapper(applicationAssembly);
			services.AddValidatorsFromAssembly(applicationAssembly)
				.AddFluentValidationAutoValidation();

			services.AddScoped<IAuthService, AuthService>();
			services.AddScoped<IOtpVoiceService, OtpVoiceService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IDesignService, DesignService>();
			services.AddScoped<IModelService, ModelService>();
			services.AddScoped<ICartService, CartService>();
			services.AddScoped<IOrderService, OrderService>();
			services.AddScoped<ICharmService, CharmService>();
			services.AddScoped<ICategoryService, CategoryService>();
			

		}
	}
}
