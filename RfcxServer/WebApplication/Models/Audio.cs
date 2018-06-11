using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApplication.Models
{

    public class Audio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AudioId { get; set; }
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string RecordingDate { get; set; }
        public string Duration { get; set; }
        public string Format { get; set; }
        public int BitRate { get; set; }
        public List<Label> LabelList { get; set; }
    }
    
}