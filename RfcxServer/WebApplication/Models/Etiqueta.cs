using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models
{
    public class Etiqueta
        {
            [BsonId]
            [BsonRepresentation(BsonType.ObjectId)] 
            public string EtiquetaId { get; set; }
            public string AudioId {get; set; }
            public int Id { get; set; }
            public string TiempoInicio { get; set; }
            public string TiempoFin { get; set; }
            public string Descripcion { get; set; }
        }
}