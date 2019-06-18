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
        public double Threshold { get; set; }
        public string Comparison{ get; set;}
        /*Status:
        CREATED: When Alert is just created because a rule is being breaking.
        SEND: When Alert is just send to Receiver
        VERIFIED: When Alert is already VERIFIED by Receiver
        CORRECT: When alert is already verified. Then server can create new alert to this station
        */
        public bool Status { get; set; }


    }

}