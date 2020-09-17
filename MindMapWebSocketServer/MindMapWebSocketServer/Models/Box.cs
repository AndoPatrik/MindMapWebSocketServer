using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MindMapWebSocketServer.Models
{
    public class Box
    {
        [JsonProperty]
        [BsonElement("boxId")]
        public string boxId { get; set; }
        [JsonProperty]
        [BsonElement("offsetLeft")]
        public int offsetLeft { get; set; }
        [JsonProperty]
        [BsonElement("offsetTop")]
        public int offsetTop { get; set; }
        [JsonProperty]
        [BsonElement("content")]
        public List<object> items { get; set; }
        [JsonProperty]
        [BsonElement("title")]
        public string title { get; set; }

        public Box()
        {

        }
        
        public Box(string boxId, int offsetLeft, int offsetTop, string title)
        {
            this.boxId = boxId;
            this.offsetLeft = offsetLeft;
            this.offsetTop = offsetTop;
            this.title = title;
        }
        
    }
}
