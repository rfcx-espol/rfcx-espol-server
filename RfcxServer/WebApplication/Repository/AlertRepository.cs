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
    public class AlertRepository : IAlertRepository
    {
        private readonly ObjectContext _context =null; 

        public AlertRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Alert>> Get()
        {
            try
            {
                return await _context.Alerts.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Alert> Get(string id)
        {
            var filter = Builders<Alert>.Filter.Eq("AlertId", id);

            try
            {
                return await _context.Alerts.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Add(Alert item)
        {
            try
            {
                item.Id=_context.Alerts.Find(_ => true).ToList().Count+1;
                await _context.Alerts.InsertOneAsync(item);
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
                DeleteResult actionResult = await _context.Alerts.DeleteOneAsync(
                        Builders<Alert>.Filter.Eq("AlertId", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        
        public async Task<bool> Update(string id, Alert item)
        {
            try
            {
                ReplaceOneResult actionResult 
                    = await _context.Alerts
                                    .ReplaceOneAsync(n => n.AlertId.Equals(id)
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
                    = await _context.Alerts.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Task<bool> UpdateLastNotification(int id, string lastNotification)
        {
            Alert alert=getAlert(id);
            alert.LastNotification= lastNotification;
            return Update(alert.AlertId, alert);
        }

        public Alert getAlert(int id){
        var filter = Builders<Alert>.Filter.Eq("Id", id);
        Alert alert=_context.Alerts.Find(filter).FirstOrDefaultAsync().Result;
        return alert;
    }

    }

    
}