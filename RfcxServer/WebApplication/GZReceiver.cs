using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace WebApplication
{
    public class GZReceiver {
        public static void HandleGZFile(IApplicationBuilder app) {
            app.Run( async (context) => {
                var msg = "gz file received \n" + context.Request.ContentType; 
                await context.Response.WriteAsync(msg);
            });
        }

        public static void HandleSendZipFile(IApplicationBuilder app) {
            app.Run( async (context) => {
                await context.Response.WriteAsync("zip file sent");
            });
        }
    }    
}

