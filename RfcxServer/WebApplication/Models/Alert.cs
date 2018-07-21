using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Alert
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertId { get; set; } 
        public int StationId { get; set; }
        public int Id { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string LastNotification { get; set; }
        /*Status:
        CREATED: When Alert is just created because a rule is being breaking.
        SEND: When Alert is just send to Receiver
        VERIFIED: When Alert is already VERIFIED by Receiver
        CORRECT: When alert is already verified. Then server can create new alert to this station
        */
        public string Status { get; set; }
        public string Repeat { get; set; }
        public string NotificationReceiver { get; set; }
        public string NotificationType { get; set; }


    }
    
}