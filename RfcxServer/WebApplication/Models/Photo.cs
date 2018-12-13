using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace WebApplication.Models
{
    public class Photo : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string PhotoId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }

        public int CompareTo(object obj)
        {
            Photo photo = obj as Photo;
            if(photo == null) return 1;
            if (photo.Id < Id)
            {
                return 1;
            }
            if (photo.Id > Id)
            {
                return -1;
            }
            return 0;
        }
    }
    
}