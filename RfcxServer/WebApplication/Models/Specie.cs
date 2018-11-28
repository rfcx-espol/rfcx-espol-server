using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Specie : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string SpecieId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public List<Photo> Gallery { get; set; }

        public int CompareTo(object obj)
        {
            Specie specie = obj as Specie;
            if(specie == null) return 1;
            if (specie.Id < Id)
            {
                return 1;
            }
            if (specie.Id > Id)
            {
                return -1;
            }
            return 0;
        }

    }
    
}