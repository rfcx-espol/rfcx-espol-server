using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Kind : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string KindId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public List<Photo> Gallery { get; set; }

        public int CompareTo(object obj)
        {
            Kind kind = obj as Kind;
            if(kind == null) return 1;
            if (kind.Id < Id)
            {
                return 1;
            }
            if (kind.Id > Id)
            {
                return -1;
            }
            return 0;
        }

    }
    
}