using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Alert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertId { get; set; }
        public int AlertsConfigurationId { get; set; }
        public int AudioId { get; set; }
        public int DataId { get; set; }
        public int Id { get; set; }
        public bool Verified { get; set; }
        public string Algorithm { get; set; }
        public string Message { get; set; }
        public bool State { get; set; }
    }
    
}