using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace WebApplication
{
    public class Startup
    {

        string icecast_config_file;
        string ices_config_file;
        readonly int stream_clients_number = 2;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            icecast_config_file = Path.Combine(Directory.GetCurrentDirectory(), "icecast", "config", "icecast.xml");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            { // Start Icecast
                Process p = new Process();
                p.StartInfo.FileName = "icecast";
                p.StartInfo.Arguments = "-c " + icecast_config_file;
                p.Start();
            }

            { // Start Ices
                for (int i = 0; i < stream_clients_number; i++)
                {

                    ices_config_file = Path.Combine(Directory.GetCurrentDirectory(), "icecast", "config", "ices-playlist-{0}.xml");
                    ices_config_file = ices_config_file.Replace("{0}", i.ToString());
                    Process p = new Process();
                    p.StartInfo.FileName = "ices";
                    p.StartInfo.Arguments = ices_config_file;
                    p.Start();
                }
            }

            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), 
                    "uploaded")));
            services.AddMvc();
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

            app.UseStaticFiles();
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
