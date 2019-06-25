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
        private readonly ObjectContext _context = null;

        public AlertRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        }


        public async Task<IEnumerable<Alert>> GetAllAlerts()
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

        public async Task<Alert> GetAlert(string id)
        {
            var filter = Builders<Alert>.Filter.Eq("Id", id);

            try
            {
                return await _context.Alerts.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task AddAlert(Alert item)
        {
            try
            {
                var list = _context.Alerts.Find(_ => true).ToList();
                if (list.Count > 0)
                {
                    item.Id = list[list.Count - 1].Id + 1;
                }
                else
                {
                    item.Id = 1;
                }

                await _context.Alerts.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveAlert(string id)
        {
            try
            {
                DeleteResult actionResult = await _context.Alerts.DeleteOneAsync(
                        Builders<Alert>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<bool> UpdateAlert(string id, Alert item)
        {
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Alerts
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


        public Alert getAlertObject(int id)
        {
            var filter = Builders<Alert>.Filter.Eq("Id", id);
            try
            {
                Alert alert = _context.Alerts.Find(filter).FirstOrDefaultAsync().Result;
                return alert;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public Condition getConditionObject(int alertId, int conditionId)
        {
            Alert alert = getAlertObject(alertId);
            Condition condition = null;
            foreach (Condition c in alert.Conditions)
            {
                if (c.Id == conditionId)
                {
                    condition = c;
                }
            }
            return condition;
        }

        public List<Alert> Get()
        {
            try
            {
                return _context.Alerts.Find(_ => true).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }


}