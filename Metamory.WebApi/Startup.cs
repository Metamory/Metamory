using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metamory.Api;
using Metamory.Api.Providers.FileSystem;
using Metamory.Api.Repositories;
using Metamory.WebApi.Policies;
using Metamory.WebApi.Policies.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
// using Microsoft.OpenApi.Models;

namespace Metamory.WebApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			// services.AddSwaggerGen(c =>
			// {
			//     c.SwaggerDoc("v1", new OpenApiInfo { Title = "Metamory", Version = "v1" });
			// });

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
					builder => builder.AllowAnyOrigin()     //TODO: Make this configurable pr. site_id
					.AllowAnyMethod()
					.AllowAnyHeader()
					// .AllowCredentials()
				);
			});

			services.AddOptions();
			services.Configure<FileRepositoryConfiguration>(Configuration.GetSection("FileRepositoryConfiguration"));

			services.AddTransient<ContentManagementService>();
			services.AddTransient<IAuthorizationPolicy, NoAuthorizationPolicy>();
			services.AddTransient<IStatusRepository, FileStatusRepository>();
			services.AddTransient<IContentRepository, FileContentRepository>();
			services.AddTransient<VersioningService>();
			services.AddTransient<CanonicalizeService>();
		}

		// // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		// public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		// {
		//     if (env.IsDevelopment())
		//     {
		//         app.UseDeveloperExceptionPage();
		//     }
		//     else
		//     {
		//         // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
		//         app.UseHsts();
		//     }

		//     app.UseHttpsRedirection();
		//     app.UseCors("CorsPolicy");
		//     // app.UseMvc();
		// }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				// app.UseSwagger();
				// app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dill v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseCors("CorsPolicy");

			// app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}
