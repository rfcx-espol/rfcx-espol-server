using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Incident
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string IncidentId { set; get; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime IncidentTime { get; set; }
        public String RaisedAlertName { get; set; }
        public String RaisedCondition { get; set; }




    }
}

