using Microsoft.AspNetCore.Http;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Vit.Extensions;

namespace Vit.AspNetCore.WebSocket
{
    public abstract class WebSocketController : IDisposable
    {

        public System.Net.WebSockets.WebSocket socket { get; private set; }
        public HttpContext HttpContext { get; private set; }

        string path;


        internal async Task InitAsync(HttpContext HttpContext, string path)
        {
            this.path = path;
            this.HttpContext = HttpContext;

            try
            {
                socket = await HttpContext.WebSockets.AcceptWebSocketAsync();

                WebSocketConnectionManager.Instance.AddController(path, this);

                //Console.WriteLine($"WebSocket连接建立：{path}- {GetConnKey()}");

                OnConnected();


                var buffer = new byte[1024 * 100];

                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                           cancellationToken: CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var text = new ArraySegment<byte>(buffer, 0, result.Count).ArraySegmentByteToString();
                        await OnGetTextAsync(text);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        return;
                    }
                }
            }
            finally
            {
                Close();
            }
        }


        public virtual string GetConnKey()
        {
            return GetHashCode().ToString();
        }


        public virtual void OnConnected()
        {
        }

        public virtual async Task OnGetTextAsync(string text)
        {
        }


        public async Task SendTextAsync(string text)
        {
            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(buffer: text.SerializeToArraySegmentByte(), messageType: WebSocketMessageType.Text,
                endOfMessage: true, cancellationToken: CancellationToken.None);
        }


        public virtual void OnClosed()
        {
        }


        public void Close()
        {
            if (socket == null) return;

            try
            {

                //Console.WriteLine($"WebSocket连接断开：{path}- {GetConnKey()}");

                lock (this)
                {
                    if (socket == null) return;
                    WebSocketConnectionManager.Instance.RemoveController(path, this);
                }

                try
                {
                    OnClosed();
                }
                catch (Exception ex)
                {
                }
            }
            finally
            {
                socket = null;
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
