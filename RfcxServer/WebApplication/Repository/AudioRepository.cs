using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;



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

    public async Task Add(Audio item)
    {
        try
        {
            await _context.Audios.InsertOneAsync(item);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Remove(string id)
    {
        try
        {
            DeleteResult actionResult = await _context.Audios.DeleteOneAsync(
                    Builders<Audio>.Filter.Eq("AudioId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Update(string id, string body)
    {
        var filter = Builders<Audio>.Filter.Eq(s => s.Id, id);
        var update = Builders<Audio>.Update
                        .Set(s => s.Body, body)
                        .CurrentDate(s => s.UpdatedOn);

        try
        {
            UpdateResult actionResult
                 = await _context.Audios.UpdateOneAsync(filter, update);

            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Update(string id, Audio item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Audios
                                .ReplaceOneAsync(n => n.Id.Equals(id)
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