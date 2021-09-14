# Vit.AspNetCore.BackgroundTask
AspNetCore后台定时任务

# (x.1)安装nuget包
``` 
// *.csproj

<ItemGroup>	 
	  <PackageReference Include="Vit.AspNetCore.WebSocket" Version="1.0.0" />
</ItemGroup>
```

# (x.2)启用

``` csharp
//Program.cs

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseWebSocketController() //启用WebSocket
                .UseStartup<Startup>();
```

# (x.3)配置定时任务
``` csharp
// WebSocketController.cs

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace App.Controller
{
    [Route("/websocket")]
    public class WebSocketController: Vit.AspNetCore.WebSocket.WebSocketController
    {       
        public override async Task OnGetTextAsync(string text)
        {
            await SendTextAsync(text);
        }
    }
}

```
