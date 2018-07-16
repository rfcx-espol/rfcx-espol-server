using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Arrays
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public List<Data> Data { get; set; }
        public string AndroidVersion { get; set; }
        public string ServicesVersion { get; set; }
        public string Name { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LastNotification { get; set; }
        }
}