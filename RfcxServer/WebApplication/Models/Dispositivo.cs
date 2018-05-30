using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{

    public class Dispositivo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string DispositivoId { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public string VersionAndroid { get; set; }
        public string VersionServicios { get; set; }
    }
    
}
