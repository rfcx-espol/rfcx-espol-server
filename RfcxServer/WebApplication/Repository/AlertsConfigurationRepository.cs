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
    public class AlertsConfigurationRepository : IAlertsConfigurationRepository
    {
        private readonly ObjectContext _context =null; 

        public AlertsConfigurationRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<AlertsConfiguration>> Get()
        {
            try
            {
                return await _context.AlertsConfigurations.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<AlertsConfiguration> Get(string id)
    {
        var filter = Builders<AlertsConfiguration>.Filter.Eq("AlertsConfigurationId", id);

        try
        {
            return await _context.AlertsConfigurations.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(AlertsConfiguration item)
    {
        try
        {
            var list=_context.AlertsConfigurations.Find(_ => true).ToList();
            if(list.Count>0){
                item.Id=list[list.Count-1].Id+1;
            }else{
                item.Id=1;
            }
            
            await _context.AlertsConfigurations.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.AlertsConfigurations.DeleteOneAsync(
                    Builders<AlertsConfiguration>.Filter.Eq("AlertsConfigurationId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    public async Task<bool> Update(string id, AlertsConfiguration item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.AlertsConfigurations
                                .ReplaceOneAsync(n => n.AlertsConfigurationId.Equals(id)
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
                = await _context.AlertsConfigurations.DeleteManyAsync(new BsonDocument());

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