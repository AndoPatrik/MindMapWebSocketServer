using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MindMapWebSocketServer.Models;
using MindMapWebSocketServer.Utility;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace WebSocketServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<WebSocketServerMiddleware> _logger;
        private WebSocketServerConnectionManager _manager;
        private MongoDb db = new MongoDb();

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketServerConnectionManager manager, ILogger<WebSocketServerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _manager = manager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                    await Receive(webSocket, async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            _logger.LogInformation($"Receive->Text -- Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                            await ProcessMessage(Encoding.UTF8.GetString(buffer, 0, result.Count), webSocket);
                            return;
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            _logger.LogInformation($"Receive->Close");
                            _manager.UnsubscribeFromProject(webSocket, _manager.GetProjectOfSocket(webSocket));
                            _manager.RemoveSocket(webSocket);
                            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                            return;
                        }
                    });
                }
                else
                {
                    _logger.LogTrace("Hello from 2nd Request Delegate - No WebSocket");
                    await _next(context);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private async Task ProcessMessage(string message, WebSocket socket)
        {
            if (Validators.IsValidJson(message))
            {
                var json = JsonConvert.DeserializeObject<dynamic>(message);

                if (message.Contains("Subscribe"))
                {
                    _manager.AddSocket(socket, json.Subscribe.ToString());
                    _manager.SubscribeToProject(socket, json.Subscribe.ToString());

                    //Temp solution -- NEED TO BE REFACTORED
                    //var yx = db.LoadProjectByName(json.Subscribe.ToString());

                    if (json.Subscribe.ToString() == "Test")
                    {
                        var localProjectState = _manager.GetLocalProjectState(json.Subscribe.ToString());

                        if (localProjectState == null)
                        {
                            var projectFromDb = db.LoadProjectById(new ObjectId("5f43cd71d5cf2d48715bd088"));
                            await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(projectFromDb)), WebSocketMessageType.Text, true, CancellationToken.None);
                            _manager.AddProjectLocalState(projectFromDb, json.Subscribe.ToString());
                            return;
                        }
                        else
                        {
                            await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(localProjectState)), WebSocketMessageType.Text, true, CancellationToken.None);
                            return;
                        }
                    }
                    if (json.Subscribe.ToString() == "TestProject2")
                    {
                        var y = db.LoadProjectById(new ObjectId("5f196da2054cb25510fdd774"));
                        await socket.SendAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(y)), WebSocketMessageType.Text, true, CancellationToken.None);
                        return;
                    }
                    else
                    {
                        string notCreatedText = "Project on this name have not been created yet!";
                        await socket.SendAsync(Encoding.UTF8.GetBytes(notCreatedText), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    return;
                }
                if (message.Contains("To"))
                {
                    //Find out what action is updated -- Done (Switch)
                    //  Update saved local object -- In progress (Body of different cases)
                    //      Send update to      

                    string projectName = json.To;
                    string action = json.Action;
                    var element = json.Element;

                    var projectToUpdate = _manager.GetLocalProjectState(projectName);
                    //var projectToUpdateDesrilazied = JsonConvert.DeserializeObject<object>(projectToUpdate.ToString());
                    string toReturn = message;

                    switch (action)
                    {
                        case "AddBox":
                            projectToUpdate.Boxes.Add(element);
                            _manager.UpdateLocalStateOfProject(projectName, projectToUpdate);
                            toReturn = message;
                            break;
                        case "RemoveBox":
                            //object ret = projectToUpdate.Boxes.Find<Box>(b => b.boxId == element.boxId);

                            //var boxes = JsonConvert.DeserializeObject<object>(projectToUpdate.Boxes.ToString());
                            //_manager.UpdateLocalStateOfProject(projectName, projectToUpdate);
                            //toReturn = message;
                            break;
                        case "MoveBox":
                            //var boxToUpdate = projectToUpdate.Boxes.Find(element.oldBox);
                            //boxToUpdate = element.newBox;
                            //_manager.UpdateLocalStateOfProject(projectName, projectToUpdate);
                            break;
                        case "AddConnection":
                            break;
                        case "RemoveConnection":
                            break;
                        case "AddInneritem":
                            break;
                        case "DraggedInneritem":
                            break;
                        case "RemoveInneritem":
                            break;
                        default:
                            toReturn = json.Message.ToString();
                            break;
                    }

                    foreach (WebSocket soc in _manager.GetSocketsFromProject(json.To.ToString()))
                    {
                        if (soc.State == WebSocketState.Open)
                        {
                            if (soc != socket)
                            {
                                await soc.SendAsync(Encoding.UTF8.GetBytes(toReturn), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                    return;
                }
            }
            else
            {
                await socket.SendAsync(Encoding.UTF8.GetBytes("Your message could not have been processed. Please try again."), WebSocketMessageType.Text, true, CancellationToken.None);
                _logger.LogWarning($"Unhandled message recieved --> {message}");
            }
        }

        private async Task Receive(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];

            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer),
                                                       cancellationToken: CancellationToken.None);

                handleMessage(result, buffer);
            }
        }
    }
}