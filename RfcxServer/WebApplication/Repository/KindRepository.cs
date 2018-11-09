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

namespace WebApplication.Repository
{
    public class KindRepository : IKindRepository
    {
        private readonly ObjectContext _context =null; 

        public KindRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public async Task<IEnumerable<Kind>> Get()
        {
            try
            {
                return await _context.Kinds.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Kind> Get(string id)
        {
            var filter = Builders<Kind>.Filter.Eq("KindId", id);

            try
            {
                return await _context.Kinds.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Kind> Get(int id)
        {
            var filter = Builders<Kind>.Filter.Eq("Id", id);

            try
            {
                return await _context.Kinds.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(Kind item)
        {
            try
            {
                var list=_context.Kinds.Find(_ => true).ToList();
                if(list.Count>0){
                    item.Id=list[list.Count-1].Id+1;
                }else{
                    item.Id=1;
                }
    
                await _context.Kinds.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int KindId)
        {
            try
            {
                DeleteResult actionResult = await _context.Kinds.DeleteOneAsync(
                        Builders<Kind>.Filter.Eq("KindId", KindId));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int KindId, Kind item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Kinds
                                    .ReplaceOneAsync(n => n.KindId.Equals(KindId)
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

        public async Task<bool> RemoveAll()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Kinds.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}