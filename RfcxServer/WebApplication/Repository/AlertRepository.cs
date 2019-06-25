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
                // var list = _context.Alerts.Find(_ => true).ToList();
                // if (list.Count > 0)
                // {
                //     item.AlertId = list[list.Count - 1].AlertId + 1;
                // }
                // else
                // {
                //     item.AlertId = 1;
                // }

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
            Alert alert = _context.Alerts.Find(filter).FirstOrDefaultAsync().Result;
            return alert;
        }

        public List<Alert> Get(){
            try {
                return _context.Alerts.Find(_ => true).ToList();
            }
            catch (Exception ex){
                throw ex;
            }
        }

        public Alert Get(int id){
            var filter = Builders<Alert>.Filter.Eq("Id", id);

            try
            {
                return _context.Alerts.Find(filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Add(Alert item){
            try
            {
                var list=_context.Alerts.Find(_ => true).ToList();
                if(item.Id==0){
                    if(list.Count>0){
                        list.Sort();
                        item.Id=list[list.Count-1].Id+1;
                    }
                    else{
                        item.Id=1;
                    } 
                }else{
                    for (int i=0;i<list.Count;i++){
                        if(item.Id==list[i].Id){
                            return false;
                        }
                    }
                }
    
                _context.Alerts.InsertOne(item);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write(ex.Source);
                Console.Write(ex.StackTrace);
                return false;
            }
        }
    }


}