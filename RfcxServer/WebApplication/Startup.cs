using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
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
            Core.MakeSpeciesFolder();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Core.FilesFolderPath));
            services.AddMvc();
            services.AddSession();

            IFileProvider physicalProvider = new PhysicalFileProvider(Core.getServerDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);
            services.Configure<Settings>(
            options =>
                {
                    options.iConfigurationRoot=Configuration;
                });
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<IAlertsConfigurationRepository, AlertsConfigurationRepository>();
            services.AddTransient<IAudioRepository, AudioRepository>();
            services.AddTransient<IStationRepository, StationRepository>();
            services.AddTransient<ILabelRepository, LabelRepository>();
            services.AddTransient<ISensorRepository, SensorRepository>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IInfoSensoresRepository, InfoSensoresRepository>();
            services.AddTransient<ISpecieRepository, SpecieRepository>();
            services.AddTransient<IPhotoRepository, PhotoRepository>();
            services.AddTransient<IQuestionRepository, QuestionRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin();
                    });
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }*/

            app.UseDeveloperExceptionPage();
            app.UseCors("AllowAllOrigins");
            app.UseMvcWithDefaultRoute();
            app.UseCookiePolicy();
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(Core.getServerDirectory(), "resources")),
                RequestPath = "/resources"
            });

            
            // app.Map("/hello", HandleHello);
            // app.Map("/sendgz", GZReceiver.HandleGZFile);
            // app.Map("/getzip", GZReceiver.HandleSendZipFile);
            // app.Run(async (context) => {
            //     await context.Response.WriteAsync("Rfcx Server is running");
            // });
            
            loggerFactory.AddSerilog();

            StaticFileOptions option = new StaticFileOptions();
            FileExtensionContentTypeProvider contentTypeProvider = (FileExtensionContentTypeProvider)option.ContentTypeProvider ?? new FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(".unityweb", "application/octet-stream");
            option.ContentTypeProvider = contentTypeProvider;
            app.UseStaticFiles(option);

        }
    }
}
