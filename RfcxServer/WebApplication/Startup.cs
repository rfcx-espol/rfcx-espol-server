using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NonFactors.Mvc.Grid;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.DbModels;
using WebApplication.IRepository;
using WebApplication.Repository;
using Serilog;
using Microsoft.Extensions.Logging;


namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = (IConfigurationRoot) configuration;
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Core.MakeFilesFolder();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Core.FilesFolderPath));
            services.AddMvc();

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);
            services.Configure<Settings>(
            options =>
                {
                    options.iConfigurationRoot=Configuration;
                });
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<IAlertsConfigurationRepository, AlertsConfigurationRepository>();
            services.AddTransient<IAudioRepository, AudioRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<ILabelRepository, LabelRepository>();
            services.AddTransient<ISensorRepository, SensorRepository>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IInfoSensoresRepository, InfoSensoresRepository>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "files")),
                RequestPath = "/files"
            });

            
            // app.Map("/hello", HandleHello);
            // app.Map("/sendgz", GZReceiver.HandleGZFile);
            // app.Map("/getzip", GZReceiver.HandleSendZipFile);
            // app.Run(async (context) => {
            //     await context.Response.WriteAsync("Rfcx Server is running");
            // });
            app.UseMvcWithDefaultRoute();
            loggerFactory.AddSerilog();
        }
    }
}