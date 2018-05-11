using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Alerta
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AlertaId { get; set; }
        public string AudioId { get; set; }
        public bool Clasificado { get; set; }
        public string Algoritmo { get; set; }
        public string Tipo { get; set; }
        public bool Estado { get; set; }
    }
    
}