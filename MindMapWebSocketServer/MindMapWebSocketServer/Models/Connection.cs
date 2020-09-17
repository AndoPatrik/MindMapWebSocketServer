using MongoDB.Bson.Serialization.Attributes;

namespace MindMapWebSocketServer.Models
{
    public class Connection
    {
        public Connection(string divA, string divB, string color, string conId)
        {
            this.divA = divA;
            this.divB = divB;
            this.color = color;
            this.conId = conId;
        }

        public Connection()
        {

        }

        [BsonElement("divA")]
        public string divA { get; set; }
        [BsonElement("divB")]
        public string divB { get; set; }
        [BsonElement("color")]
        public string color { get; set; }
        [BsonElement("connectionID")]
        public string conId { get; set; }
    }
}
