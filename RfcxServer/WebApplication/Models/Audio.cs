using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace WebApplication.Models
{

    public class Audio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string AudioId { get; set; }
        public int DispositivoId { get; set; }
        public int Id { get; set; }
        public string FechaLlegada { get; set; }
        public string FechaGrabacion { get; set; }
        public string Duracion { get; set; }
        public string Formato { get; set; }
        public int BitRate { get; set; }
        public List<Etiqueta> EtiquetasList { get; set; }
    }
    
}