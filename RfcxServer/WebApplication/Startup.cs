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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NonFactors.Mvc.Grid;
using Microsoft.EntityFrameworkCore;
using WebApplication.Models;
using WebApplication.DbModels;
using WebApplication.IRepository;
using WebApplication.Repository;


namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /** 
        public void ConfigureServices(IServiceCollection services)
        {
            Core.MakeFilesFolder();
            services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Core.FilesFolderPath));
            services.AddMvc();

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            services.AddSingleton<IFileProvider>(physicalProvider);

            services.AddDbContext<BosqueContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BosqueDatabase")));
         
        }
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddMvc();
                services.Configure<Settings>(
                options =>
                    {
                        options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                        options.Database = Configuration.GetSection("MongoConnection:Database").Value;
                    });
            }
        */ 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<Settings>(
            options =>
                {
                    options.iConfigurationRoot=Configuration;
                });
            services.AddTransient<IAlertaRepository, AlertaRepository>();
            services.AddTransient<IAlertaConfiguracionRepository, AlertaConfiguracionRepository>();
            services.AddTransient<IAudioRepository, AudioRepository>();
            services.AddTransient<IDispositivoRepository, DispositivoRepository>();
            services.AddTransient<IEtiquetaRepository, EtiquetaRepository>();
            services.AddTransient<ISensorRepository, SensorRepository>();
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
        }
    }
}
