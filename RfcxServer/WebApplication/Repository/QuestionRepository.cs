using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Driver;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;

namespace WebApplication.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly ObjectContext _context =null; 

        public QuestionRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public List<Question> Get()
        {
            try
            {
                return _context.Questions.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Question> Get(string id)
        {
            var filter = Builders<Question>.Filter.Eq("QuestionId", id);

            try
            {
                return await _context.Questions.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Question Get(int id)
        {
            var filter = Builders<Question>.Filter.Eq("Id", id);

            try
            {
                return _context.Questions.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Add(Question item)
        {
            try
            {
                var list=_context.Questions.Find(_ => true).ToList();
                if(item.Id==0){
                    if(list.Count>0){
                        list.Sort();
                        item.Id=list[list.Count-1].Id+1;
                    }
                    else{
                        item.Id=1;
                    } 
                }else{
                    for (int i=0;i<list.Count;i++){
                        if(item.Id==list[i].Id){
                            return false;
                        }
                    }
                }
    
                _context.Questions.InsertOne(item);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.Source);
                Console.Write(ex.StackTrace);
                return false;
            }
        }

        public bool Remove(int id)
        {
            try
            {
                var filtro_pregunta = Builders<Question>.Filter.Eq("Id", id);
                DeleteResult actionResult = _context.Questions.DeleteOne(filtro_pregunta);
                return actionResult.IsAcknowledged 
                        && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                Console.Write("error: " + ex.StackTrace + "\n");
                Console.Write("error: " + ex.Message + "\n");
                throw ex;
            }
        }

        public bool Update(int id, Question item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = _context.Questions
                                    .ReplaceOne(n => n.Id.Equals(id)
                                            , item
                                            , new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateSpecieId(int id, int specie_id)
        {
            Question question = Get(id);
            question.SpecieId = specie_id;
            return Update(id, question);
        }

        public bool UpdateText(int id, string text)
        {
            var filter = Builders<Question>.Filter.Eq("Id", id);
            Question question  = _context.Questions.Find(filter).FirstOrDefault();
            question.Text = text;
            return Update(id, question);
        }

        public bool UpdateOption(int id, int index, string option)
        {
            Question question = Get(id);
            (question.Options)[index] = option;
            return Update(id, question);
        }

        public bool UpdateAnswer(int id, int answer)
        {
            Question question = Get(id);
            question.Answer = answer;
            return Update(id, question);
        }

        public bool UpdateFeedback(int id, string feedback)
        {
            Question question = Get(id);
            question.Feedback = feedback;
            return Update(id, question);
        }

    }

}