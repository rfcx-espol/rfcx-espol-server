using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;



namespace WebApplication.Repository
{
    public class DispositivoRepository : IDispositivoRepository
    {
        private readonly ObjectContext _context =null; 

        public DispositivoRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Dispositivo>> Get()
        {
            try
            {
                return await _context.Dispositivos.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Dispositivo> Get(string id)
    {
        var filter = Builders<Dispositivo>.Filter.Eq("DispositivoId", id);

        try
        {
            return await _context.Dispositivos.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Dispositivo item)
    {
        try
        {
            await _context.Dispositivos.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Dispositivos.DeleteOneAsync(
                    Builders<Dispositivo>.Filter.Eq("DispositivoId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    

    public async Task<bool> Update(string id, Dispositivo item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Dispositivos
                                .ReplaceOneAsync(n => n.DispositivoId.Equals(id)
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
                = await _context.Dispositivos.DeleteManyAsync(new BsonDocument());

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