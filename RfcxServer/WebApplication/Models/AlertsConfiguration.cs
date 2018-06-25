using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class AlertsConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        //If SensorId!=null then var1=stationId var2=sensorid  get station/var1/sensor/var2/lastdata, else var1=stationId
        public string AlertsConfigurationId { get; set; }
        public int Id { get; set; }
        public int StationId { get; set; }
        public int SensorId { get; set; }
        public string Action { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public string NotificationReceiver { get; set; }
        public string Repeat { get; set; }
        //In Binary Values(Only have 2 states), should be 0, and a different value could be an alert
        public string Value { get; set; }

        public string Units { get; set; }
        
        public string MinValue { get;  set; }
        public string MaxValue { get; set; }
        public string Status {get; set; }
    
    }
    
}
