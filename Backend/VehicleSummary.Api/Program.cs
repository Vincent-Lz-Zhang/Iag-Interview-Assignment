using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VehicleSummary.Api.Models;
using VehicleSummary.Api.Services.Interfaces;
using VehicleSummary.Api.Services.PollyPolicy;
using VehicleSummary.Api.Services.ResilientApiClient;
using VehicleSummary.Api.Services.VehicleSummary;
using VehicleSummary.Api.ServicesInterfaces;

namespace VehicleSummary.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton<IPollyPolicies, PollyPolicies>();
            builder.Services.AddScoped<IIagResilientApiClient, IagResilientApiClient>();
            builder.Services.AddScoped<IVehicleSummaryService, VehicleSummaryService>();
            builder.Services.Configure<VehicleApiOptions>(
                builder.Configuration.GetSection("VehicleApiOptions"));

            builder.Services.AddApiVersioning(setupAction =>
            {
                setupAction.AssumeDefaultVersionWhenUnspecified = true;
                setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                setupAction.ReportApiVersions = true;
            });

            var frontendOrigin = builder.Configuration.GetValue<string>("FrontendOrigin");

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: ApiConstants.AllowSpecificOrigin,
                                  policy =>
                                  {
                                      policy.WithOrigins(frontendOrigin);
                                  });
            });

            builder.Services.AddControllers();

            ConfigFlurl(builder.Services);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseSwagger();
                //app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }

        /// <remarks>
        /// It is supposed to result in an extra container, which could be problematic.
        /// </remarks>
        private static void ConfigFlurl(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider()
                .CreateScope().ServiceProvider;

            var pollyPolicies = serviceProvider.GetRequiredService<IPollyPolicies>();

            FlurlHttp.Configure(settings => settings.HttpClientFactory = new PollyHttpClientFactory(pollyPolicies));
        }

    }
}
