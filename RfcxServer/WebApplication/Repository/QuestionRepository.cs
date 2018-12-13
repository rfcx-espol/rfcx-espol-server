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

        public async Task<IEnumerable<Question>> Get()
        {
            try
            {
                return await _context.Questions.Find(_ => true).ToListAsync();
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

        public async Task<Question> Get(int id)
        {
            var filter = Builders<Question>.Filter.Eq("Id", id);

            try
            {
                return await _context.Questions.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Add(Question item)
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
    
                await _context.Questions.InsertOneAsync(item);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.Source);
                Console.Write(ex.StackTrace);
                throw ex;
            }
        }

    }

}