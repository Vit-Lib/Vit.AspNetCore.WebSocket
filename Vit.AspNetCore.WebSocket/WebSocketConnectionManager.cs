using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Vit.AspNetCore.WebSocket
{

    public class WebSocketConnectionManager
    {

        public static readonly WebSocketConnectionManager Instance=new WebSocketConnectionManager();


        // path => socketList
        private ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocketController>> socketMap =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, WebSocketController>>();

        

        public WebSocketController GetController(string path,string connKey)
        {
            if (socketMap.TryGetValue(path, out var map)) 
            {
                if(map.TryGetValue(connKey,out var controller))return controller;
            }
            return null;
        }



        public ICollection<WebSocketController> GetAll(string path)
        {
            if (socketMap.TryGetValue(path, out var map))
            {
                return map.Values;
            }
            return null;
        }

        public WebSocketController RemoveController(string path, WebSocketController controller)
        {
            if (socketMap.TryGetValue(path, out var map))
            { 
                if (map.TryRemove(controller.GetConnKey(), out var controller_)) return controller_;
            }
            return null;
        }


        public void AddController(string path, WebSocketController controller)
        {
            if (!socketMap.TryGetValue(path, out var map))
            {
                socketMap[path] = map = new ConcurrentDictionary<string, WebSocketController>();
            }

            map[controller.GetConnKey()] = controller;
        }
        


    }

     
}
