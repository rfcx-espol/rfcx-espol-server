using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;


namespace WebApplication.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly ObjectContext _context =null; 

        public DataRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Data>> Get()
        {
            try
            {
                return await _context.Datas.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Data> Get(string id)
    {
        var filter = Builders<Data>.Filter.Eq("DataId", id);

        try
        {
            return await _context.Datas.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<Data> Get(int id)
    {
        var filter = Builders<Data>.Filter.Eq("Id", id);

        try
        {
            return await _context.Datas.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<IEnumerable<Data>> GetByDispositivoSensor(int DispositivoId, int SensorId)
    {
        try
        {
            var filter =Builders<Data>.Filter.Eq("DispositivoId", DispositivoId) & Builders<Data>.Filter.Eq("SensorId", SensorId);
            return await _context.Datas.Find(filter).ToListAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<Data> Get(int DispositivoId, int SensorId, int DataId)
    {
        var filter = Builders<Data>.Filter.Eq("Id", DataId) & Builders<Data>.Filter.Eq("DispositivoId", DispositivoId) & Builders<Data>.Filter.Eq("SensorId", SensorId);

        try
        {
            return await _context.Datas.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public async Task Add(Data item)
    {
        try
        {
            item.Id=_context.Datas.Find(_ => true).ToList().Count+1;
            await _context.Datas.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Datas.DeleteOneAsync(
                    Builders<Data>.Filter.Eq("DataId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    public async Task<bool> Update(string id, Data item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Datas
                                .ReplaceOneAsync(n => n.DataId.Equals(id)
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
                = await _context.Datas.DeleteManyAsync(new BsonDocument());

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