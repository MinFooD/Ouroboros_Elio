using BusinessLogicLayer.Dtos.MailUtils;
using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ouroboros.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static void AddPresentationLayer(this WebApplicationBuilder builder)
		{
			builder.Services.AddControllersWithViews();
	//		builder.Services.AddSwaggerGen(options =>
	//		{
	//			options.SwaggerDoc("v1", new OpenApiInfo { Title = "Ouroboros", Version = "v1" });
	//			options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
	//			{
	//				Name = "Authorization",
	//				In = ParameterLocation.Header,
	//				Type = SecuritySchemeType.ApiKey,
	//				Scheme = JwtBearerDefaults.AuthenticationScheme
	//			});

	//			options.AddSecurityRequirement(new OpenApiSecurityRequirement
	//{
	//	{
	//		new OpenApiSecurityScheme
	//		{
	//			Reference = new OpenApiReference
	//			{
	//				Type = ReferenceType.SecurityScheme,
	//				Id = JwtBearerDefaults.AuthenticationScheme
	//			},
	//			Scheme = "Oauth2",
	//			Name = JwtBearerDefaults.AuthenticationScheme,
	//			In = ParameterLocation.Header
	//		},
	//		new List<string>()
	//	}
	//});
	//		});

				builder.Services.AddIdentityCore<ApplicationUser>()
			.AddRoles<IdentityRole<Guid>>()
			.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Ouroboros")
			.AddEntityFrameworkStores<OuroborosContext>()
			.AddDefaultTokenProviders();
			builder.Services.AddStackExchangeRedisCache(options =>
			{
				options.Configuration = "localhost:7111";  
				options.InstanceName = "Ouroboros";    
			});

			//var mailsettings = builder.Configuration.GetSection("MailSettings"); // đọc config
			//builder.Services.Configure<MailSettings>(mailsettings);

			//	builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			//.AddJwtBearer(option =>
			//option.TokenValidationParameters = new TokenValidationParameters
			//{
			//	ValidateIssuer = true,
			//	ValidateAudience = true,
			//	ValidateLifetime = true,
			//	ValidateIssuerSigningKey = true,
			//	ValidIssuer = builder.Configuration["Jwt:Issuer"],
			//	ValidAudience = builder.Configuration["Jwt:Audience"],
			//	IssuerSigningKey = new SymmetricSecurityKey(
			//		Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"]))
			//});

			//	builder.Services.AddEndpointsApiExplorer();
		}
	}
}
