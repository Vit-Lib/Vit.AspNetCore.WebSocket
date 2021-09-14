using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Vit.AspNetCore.WebSocket
{
    public class WebSocketMiddleware 
      
    {
        private readonly RequestDelegate _next;
        private readonly string path;
        private readonly Type WebSocketController;

        public WebSocketMiddleware(RequestDelegate next,string path,Type WebSocketController)
        {
            _next = next;
            this.path = path;
            this.WebSocketController = WebSocketController;
        }

        public async Task InvokeAsync(HttpContext context)
        {  
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocketController controller = context.RequestServices.GetService(WebSocketController) as WebSocketController;
                if (controller != null)
                {
                    await controller.InitAsync(context, path);
                    return;
                }
            }

            await _next(context);
            return;
        }


    }

     
}
