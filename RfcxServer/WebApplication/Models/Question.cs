using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    
    public class Question : IComparable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string QuestionId { get; set; }
        public int Id { get; set; }
        public int SpecieId { get; set; }
        public string Text { get; set; }
        public List<string> Options { get; set; }
        public int Answer { get; set; }
        public string Feedback { get; set; }

        public int CompareTo(object obj)
        {
            Question question = obj as Question;
            if(question == null) return 1;
            if (question.Id < Id)
            {
                return 1;
            }
            if (question.Id > Id)
            {
                return -1;
            }
            return 0;
        }

    }
    
}