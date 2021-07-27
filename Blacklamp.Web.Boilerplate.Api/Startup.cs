using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Blacklamp.Web.Boilerplate.Api.Directory;
using Blacklamp.Web.Boilerplate.Infrastructure.Extensions;
using Blacklamp.Web.Boilerplate.Infrastructure.Identity.Models.Authentication;
using MediatR;

namespace Blacklamp.Web.Boilerplate.Api
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddAutoMapper(typeof(Startup));
			services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);
			services.AddControllers();
			services.Configure<TokenSettings>(Configuration.GetSection("token"));
			services.SetupIdentityDatabase(Configuration);
			services.SetupSwaggerDoc(Configuration);
			services.AddHttpContextAccessor();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blacklamp.Web.Boilerplate.Api v1"));
				
				app.EnsureIdentityDbIsCreated();
				app.SeedIdentityDataAsync().Wait();
			}

			app.UseHttpsRedirection();

			app.UseCors();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
