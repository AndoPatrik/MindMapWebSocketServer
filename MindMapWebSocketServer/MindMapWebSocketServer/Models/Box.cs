using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MindMapWebSocketServer.Models
{
    public class Box
    {
        [JsonProperty]
        public string BoxId { get; set; }
        [JsonProperty]
        public int OffsetLeft { get; set; }
        [JsonProperty]
        public int OffsetTop { get; set; }
        [JsonProperty]
        public List<object> Connections { get; set; }
        [JsonProperty]
        public List<object> Boxes { get; set; }
        [JsonProperty]
        public List<object> Items { get; set; }
    }
}
