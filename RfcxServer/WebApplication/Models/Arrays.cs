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
    }
}