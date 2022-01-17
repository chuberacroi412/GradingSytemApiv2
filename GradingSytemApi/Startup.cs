using GradingSytemApi.Common.Helpers;
using GradingSytemApi.Entities;
using GradingSytemApi.Extentions;
using GradingSytemApi.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradingSytemApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Settings.Configuration = configuration;
            JWTHelper.Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(config => config.AddPolicy("CorsPolicy", builder => builder.AllowAnyHeader()
                                                                                        .AllowAnyOrigin()
                                                                                        .AllowAnyMethod()
                                                                                        .SetIsOriginAllowed(origin => true)));

            services.AddControllers();
            services.AddCustomDbContext();
            services.AddCustomIdentity();
            services.AddJwt();
            services.AddCustomSwagger();
            services.AddCustomErrorLocalization();
            services.RegisterCustomService();
            services.RegisterApiServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ApiDbContext apiDbContext, IWebHostEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment()) 
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "DefaultAPI",
                    pattern: "api/{controller}/{action}/{id?}");
            });

            // ===== Setup Swagger =====
            app.UseSwagger();
            app.UseSwaggerUI(config =>
            {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api v1");
            });

            // ===== Create Database =====
            try
            {
                apiDbContext.Database.Migrate();
            }
            catch(SqlException e)
            {

            }

            // ===== Default values =====
            GenerateDefaultValue.GenerateData(services, env, Configuration.GetConnectionString("ApiConnection")).Wait();
        }
    }
}
