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
