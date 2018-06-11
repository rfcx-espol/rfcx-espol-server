using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class AlertsConfiguration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertsConfigurationId { get; set; }
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public string Mail { get; set; }
        //En audio MinValue y MaxValue deberían ser 0, y un valor diferente sería alerta
        public string MinValue { get;  set; }
        public string MaxValue { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string SleepTime { get; set; }

    
    }
    
}
