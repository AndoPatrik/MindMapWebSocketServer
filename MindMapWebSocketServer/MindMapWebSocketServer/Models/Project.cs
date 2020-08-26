using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace MindMapWebSocketServer.Models
{
    public class Project
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("boxes")]
        public List<object> Boxes { get; set; }
        [BsonElement("connections")]
        public List<object> Connections { get; set; }
        [BsonElement("name")]
        public string ProjectName { get; set; }
    }
}
