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
    public class AudioRepository : IAudioRepository
    {
        private readonly ObjectContext _context =null; 

        public AudioRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Audio>> Get()
        {
            try
            {
                return await _context.Audios.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Audio> Get(string id)
    {
        var filter = Builders<Audio>.Filter.Eq("AudioId", id);

        try
        {
            return await _context.Audios.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<Audio> Get(int id)
    {
        var filter = Builders<Audio>.Filter.Eq("Id", id);

        try
        {
            return await _context.Audios.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

        public async Task<IEnumerable<Audio>> GetByDispositivo(int DispositivoId)
        {
            try
            {
                var filter =Builders<Audio>.Filter.Eq("DispositivoId", DispositivoId);
                return await _context.Audios.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<Audio> Get(int DispositivoId, int AudioId)
        {
            var filter = Builders<Audio>.Filter.Eq("Id", AudioId) & Builders<Audio>.Filter.Eq("DispositivoId", DispositivoId);

            try
            {
                return await _context.Audios.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(Audio item)
        {
            try
            {
                item.Id=_context.Audios.Find(_ => true).ToList().Count+1;
                await _context.Audios.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Remove(int DispositivoId, int AudioId)
        {
            try
            {
                DeleteResult actionResult = await _context.Audios.DeleteOneAsync(
                        Builders<Audio>.Filter.Eq("AudioId", AudioId));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Update(int DispositivoId, int AudioId, Audio item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Audios
                                    .ReplaceOneAsync(n => n.AudioId.Equals(AudioId)
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
                    = await _context.Audios.DeleteManyAsync(new BsonDocument());

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