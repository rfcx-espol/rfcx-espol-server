using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Condition
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConditionId { get; set; }
        public int Id { get; set; }
        public int SensorId { get; set; }
        public double Threshold { get; set; }
        public string Comparison { get; set; }

        // public bool Status { get; set; }


    }

}