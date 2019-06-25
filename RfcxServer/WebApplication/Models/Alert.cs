using System.Collections.Generic;
//using static WebApplication.Models.Condition;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Alert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertId { get; set; }
        public int Id { get; set; }
        public int StationId { get; set; }
        public string Name { get; set; }
        public string AlertType { get; set; }
        public List<string> Mailto { get; set; }
        public string Message { get; set; }
        public List<Condition> Conditions { get; set; }
        public bool Status { get; set; }


    }

}