using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace WebSocketServer
{
    public class WebSocketServerConnectionManager
    {
        private ConcurrentDictionary<WebSocket, string> _sockets = new ConcurrentDictionary<WebSocket, string>();
        private ConcurrentDictionary<string, List<WebSocket>> _workspaceGroups = new ConcurrentDictionary<string, List<WebSocket>>();

        public void AddSocket(WebSocket socket, string workspaceName)
        {
            _sockets.TryAdd(socket, workspaceName);
            Console.WriteLine($"--- Socket saved");
        }

        public void RemoveSocket(WebSocket socket) 
        {
            string removed;
            _sockets.TryRemove(socket, out removed);
            Console.WriteLine($"--- Socket removed");
        }

        public string GetWorkspaceOfSocket(WebSocket socket) 
        {
            return _sockets.FirstOrDefault(x => x.Key == socket).Value;
        }

        public void SubscribeToWorkspace(WebSocket socket, string workspaceName) 
        {
            if (!_workspaceGroups.ContainsKey(workspaceName))
            {
                _workspaceGroups.TryAdd(workspaceName, new List<WebSocket>());
            }

            var workspace = _workspaceGroups.FirstOrDefault(x => x.Key == workspaceName);
            workspace.Value.Add(socket);
            Console.WriteLine($"--- New client subscribed to: {workspaceName}. # of subscribers: {workspace.Value.Count}");
        }

        public List<WebSocket> GetSocketsFromWorkspace(string workspaceName) 
        {
            var workspace = _workspaceGroups.FirstOrDefault(x => x.Key == workspaceName);
            return workspace.Value;
        }

        public void UnsubscribeFromWorkspace(WebSocket socket, string workspaceName)
        {
            var workspace = _workspaceGroups.FirstOrDefault(x => x.Key == workspaceName);
            workspace.Value.Remove(socket);
            Console.WriteLine($"--- Client unsubscribed from: {workspaceName}. # of subscribers: {workspace.Value.Count}");
        }
    }
}