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
    public class PhotoRepository : IPhotoRepository
    {
        private readonly ObjectContext _context =null; 

        public PhotoRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 

        public async Task<IEnumerable<Photo>> Get()
        {
            try
            {
                return await _context.Photos.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Photo> Get(string id)
        {
            var filter = Builders<Photo>.Filter.Eq("PhotoId", id);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Photo> Get(int id)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", id);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Photo>> GetByKind(int KindId)
        {
            try
            {
                var filter =Builders<Photo>.Filter.Eq("KindId", KindId);
                return await _context.Photos.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Photo> Get(int KindId, int PhotoId)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", PhotoId) & Builders<Photo>.Filter.Eq("KindId", KindId);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task Add(Photo item)
        {
            try
            {
                var list=_context.Photos.Find(_ => true).ToList();
                if(list.Count>0){
                    item.Id=list[list.Count-1].Id+1;
                }else{
                    item.Id=1;
                }
    
                await _context.Photos.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int PhotoId)
        {
            try
            {
                DeleteResult actionResult = await _context.Photos.DeleteOneAsync(
                        Builders<Photo>.Filter.Eq("PhotoId", PhotoId));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int PhotoId, Photo item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Photos
                                    .ReplaceOneAsync(n => n.PhotoId.Equals(PhotoId)
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
                    = await _context.Photos.DeleteManyAsync(new BsonDocument());

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