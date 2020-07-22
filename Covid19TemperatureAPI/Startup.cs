using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using Covid19TemperatureAPI.Entities.Data;
using Covid19TemperatureAPI.Entities.Models;
using Covid19TemperatureAPI.SignalRHub;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Covid19TemperatureAPI
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
            //SeedData.EnsureSeedData(Configuration["SqlServerConnectionString"]);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration["SqlServerConnectionString"]), ServiceLifetime.Transient);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();

            services.AddSingleton<IConfiguration>(Configuration);

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddDeveloperSigningCredential()
                .AddInMemoryApiResources(Config.GetApis(Configuration["APIName"], Configuration["ClientSecret"]))
                .AddInMemoryClients(Config.GetClients(Configuration["APIClientId"], Configuration["ClientSecret"], Configuration["APIName"]))
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = Configuration["TokenAuthService"];
                    options.RequireHttpsMetadata = false;
                    options.ApiName = Configuration["APIName"];
                    options.ApiSecret = Configuration["ClientSecret"];
                    //options.EnableCaching = true;
                    //options.CacheDuration = TimeSpan.FromSeconds(1);
                    options.EnableCaching = false;
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Covid-19 Temperature Rest API", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TEMPERATURE API V1");
            });

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseIdentityServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<Covid19Hub>(Configuration["SignalRHubRelativeUrl"]);
            });

            app.UseMvcWithDefaultRoute();
            //app.UseHttpsRedirection();
            //app.UseMvc();
        }
    }
}
