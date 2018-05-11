using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class AlertaConfiguracion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertaConfiguracionId { get; set; }
        public string Correo { get; set; }
        public string Frecuencia {get; set; }
        public string Repeticiones { get; set; }
    
    }
    
}
