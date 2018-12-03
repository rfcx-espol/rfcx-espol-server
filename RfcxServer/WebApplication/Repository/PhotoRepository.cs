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

        public async Task<IEnumerable<Photo>> GetBySpecie(int SpecieId)
        {
            try
            {
                var filter =Builders<Photo>.Filter.Eq("SpecieId", SpecieId);
                return await _context.Photos.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Photo> Get(int SpecieId, int PhotoId)
        {
            var filter = Builders<Photo>.Filter.Eq("Id", PhotoId) & Builders<Photo>.Filter.Eq("SpecieId", SpecieId);

            try
            {
                return await _context.Photos.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void Add(Photo item)
        {
            try
            {
                var list=_context.Photos.Find(_ => true).ToList();
                if(list.Count>0){
                    item.Id=list[list.Count-1].Id+1;
                }else{
                    item.Id=1;
                }
    
                 _context.Photos.InsertOne(item);
                 return;
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
                        Builders<Photo>.Filter.Eq("Id", PhotoId));

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
                                    .ReplaceOneAsync(n => n.Id.Equals(PhotoId)
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

        public Task<bool> UpdateDescription(int PhotoId, string description)
        {
            Photo photo = getPhoto(PhotoId);
            photo.Description = description;
            return Update(photo.Id, photo);        
        }

        public Photo getPhoto(int id){
            var filter = Builders<Photo>.Filter.Eq("Id", id);
            Photo photo = _context.Photos.Find(filter).FirstOrDefaultAsync().Result;
            return photo;
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