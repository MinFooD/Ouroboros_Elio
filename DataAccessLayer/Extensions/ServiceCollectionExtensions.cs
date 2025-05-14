using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
		{
			//var connectionString = configuration.GetConnectionString("DefaultConnection");
			//services.AddDbContext<OuroborosContext>(options => options.UseSqlServer(connectionString));

			services.AddScoped<IDesignRepository, DesignRepository>();
		}
	}
}
