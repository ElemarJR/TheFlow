using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;
using TheFlow.Notifications.Application;
using TheFlow.Notifications.WebApi.Infrastructure;

namespace TheFlow.Notifications.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")),
                    TimeSpan.Zero
                );
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "TheFlow Notification API",
                    Version = "v1"
                });

                options.OperationFilter<ExamplesOperationFilter>();
                options.OperationFilter<DescriptionOperationFilter>();
            });

            return services.AddApplication();
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseSwagger()
                .UseSwaggerUI(setup =>
                {
                    setup.SwaggerEndpoint("/swagger/v1/swagger.json", "TheFlow Notification API v1");
                });

            //app.UseMvc();
            app.UseMvcWithDefaultRoute();
        }
    }
}
