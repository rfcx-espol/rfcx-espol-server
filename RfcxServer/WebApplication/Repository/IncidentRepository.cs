using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.DbModels;
using WebApplication.IRepository;
using WebApplication.Models;

namespace WebApplication.Repository
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly ObjectContext _context = null;

        public IncidentRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        }

        public async Task AddIncident(Incident item)
        {
            try
            {
                await _context.Incident.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Incident>> GetAllIncidents()
        {
            try
            {
                return await _context.Incident.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Incident> GetIncident(string id)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));

            try
            {
                return await _context.Incident.Find(filter).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> RemoveIncident(string id)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));
            try
            {
                DeleteResult actionResult = await _context.Incident.DeleteOneAsync(filter);

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateIncident(string id, Incident item)
        {
            var filter = Builders<Incident>.Filter.Eq("_id", ObjectId.Parse(id));
            try
            {
                ReplaceOneResult actionResult
                    = await _context.Incident.ReplaceOneAsync(filter, item, new UpdateOptions { IsUpsert = true });
                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}