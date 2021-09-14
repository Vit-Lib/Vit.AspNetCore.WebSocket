
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Linq; 
using Microsoft.AspNetCore.Hosting;
using System;
using Vit.AspNetCore.WebSocket;

namespace Vit.Extensions
{
    public static class IWebHostBuilder_UseWebSocketController_Extensions
    {


        public static IWebHostBuilder UseWebSocketController(this IWebHostBuilder hostBuilder, double KeepAliveInterval = 20)
        {
            var types = Assembly.GetEntryAssembly().ExportedTypes.Where(t => typeof(WebSocketController).IsAssignableFrom(t) && t != typeof(WebSocketController)).ToList();

            #region (x.2)Injection IServiceCollection
            {
                hostBuilder.ConfigureServices(services =>
                {
                    foreach (var type in types)
                    {
                        services.AddScoped(type);
                    }
                });
            }
            #endregion


            #region (x.3)Injection IApplicationBuilder
            {
                hostBuilder.ConfigureServices((IServiceCollection services) =>
                {
                    services.AddTransient<IStartupFilter>(m =>
                    {
                        return new AutoRequestServicesStartupFilter
                        {
                            applicationBuilder = app =>
                            {

                                var webSocketOptions = new WebSocketOptions()
                                {
                                    KeepAliveInterval = TimeSpan.FromSeconds(KeepAliveInterval),
                                    ReceiveBufferSize = 4 * 1024
                                };

                                app.UseWebSockets(webSocketOptions);

                                foreach (var type in types)
                                {
                                    var attr = type.GetCustomAttribute<Microsoft.AspNetCore.Mvc.RouteAttribute>();
                                    if (attr == null) continue;
                                    var path = attr.Template;
                                    app.Map(path, (_app) => _app.UseMiddleware<WebSocketMiddleware>(path, type));
                                }

                            }
                        };
                    });
                });
            }
            #endregion

            return hostBuilder;
        }



        #region AutoRequestServicesStartupFilter
        private class AutoRequestServicesStartupFilter : IStartupFilter
        {
            public Action<IApplicationBuilder> applicationBuilder;
            public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
            {
                return builder =>
                {
                    applicationBuilder.Invoke(builder);
                    next(builder);
                };
            }
        }
        #endregion
    }


}
