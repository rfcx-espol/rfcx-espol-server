using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{

    public class Device
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DeviceId { get; set; }
        public int Id { get; set; }
        public string APIKey { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string AndroidVersion { get; set; }
        public string ServicesVersion { get; set; }
    }
    
}
