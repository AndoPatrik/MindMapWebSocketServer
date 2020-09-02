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
        private ConcurrentDictionary<string, List<WebSocket>> _projectGroups = new ConcurrentDictionary<string, List<WebSocket>>();

        public void AddSocket(WebSocket socket, string projectName)
        {
            _sockets.TryAdd(socket, projectName);
            Console.WriteLine($"--- Socket saved");
        }

        public void RemoveSocket(WebSocket socket) 
        {
            string removed;
            _sockets.TryRemove(socket, out removed);
            Console.WriteLine($"--- Socket removed");
        }

        public string GetProjectOfSocket(WebSocket socket) 
        {
            return _sockets.FirstOrDefault(x => x.Key == socket).Value;
        }

        public void SubscribeToProject(WebSocket socket, string projectName) 
        {
            if (!_projectGroups.ContainsKey(projectName))
            {
                _projectGroups.TryAdd(projectName, new List<WebSocket>());
            }

            var project = _projectGroups.FirstOrDefault(x => x.Key == projectName);
            project.Value.Add(socket);
            Console.WriteLine($"--- New client subscribed to: {projectName}. # of subscribers: {project.Value.Count}");
        }

        public List<WebSocket> GetSocketsFromProject(string projectName) 
        {
            var workspace = _projectGroups.FirstOrDefault(x => x.Key == projectName);
            return workspace.Value;
        }

        public void UnsubscribeFromProject(WebSocket socket, string workspaceName)
        {
            var workspace = _projectGroups.FirstOrDefault(x => x.Key == workspaceName);
            workspace.Value.Remove(socket);
            Console.WriteLine($"--- Client unsubscribed from: {workspaceName}. # of subscribers: {workspace.Value.Count}");
        }
    }
}