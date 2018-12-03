using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication.Models
{
    public class Photo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PhotoId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
    }
    
}