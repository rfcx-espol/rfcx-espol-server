using WebApplication.DbModels;
using WebApplication.IRepository;
using Microsoft.Extensions.Options;
using WebApplication.Models;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace WebApplication.Repository
{
    public class AlertaConfiguracionRepository : IAlertaConfiguracionRepository
    {
        private readonly ObjectContext _context =null; 

        public AlertaConfiguracionRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<AlertaConfiguracion>> Get()
        {
            try
            {
                return await _context.AlertaConfiguracions.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<AlertaConfiguracion> Get(string id)
    {
        var filter = Builders<AlertaConfiguracion>.Filter.Eq("AlertaConfiguracionId", id);

        try
        {
            return await _context.AlertaConfiguracions.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(AlertaConfiguracion item)
    {
        try
        {
            await _context.AlertaConfiguracions.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.AlertaConfiguracions.DeleteOneAsync(
                    Builders<AlertaConfiguracion>.Filter.Eq("AlertaConfiguracionId", id));

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
        var filter = Builders<AlertaConfiguracion>.Filter.Eq(s => s.Id, id);
        var update = Builders<AlertaConfiguracion>.Update
                        .Set(s => s.Body, body)
                        .CurrentDate(s => s.UpdatedOn);

        try
        {
            UpdateResult actionResult
                 = await _context.AlertaConfiguracions.UpdateOneAsync(filter, update);

            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task<bool> Update(string id, AlertaConfiguracion item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.AlertaConfiguracions
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
                = await _context.AlertaConfiguracions.DeleteManyAsync(new BsonDocument());

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