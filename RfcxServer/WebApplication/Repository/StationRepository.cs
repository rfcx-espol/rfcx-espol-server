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
    public class StationRepository : IStationRepository
    {
        private readonly ObjectContext _context =null; 

        public StationRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Station>> Get()
        {
            try
            {
                return await _context.Stations.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Station> Get(string apiKey)
    {
        var filter = Builders<Station>.Filter.Eq("APIKey", apiKey);

        try
        {
            if(_context.Stations.Find(filter).ToList().Count==0){
                return null;
            }
            return await _context.Stations.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public int GetStationCount(string apiKey){
        var filter = Builders<Station>.Filter.Eq("APIKey", apiKey);
        int count = _context.Stations.Find(filter).ToList().Count;
        return count;
    }

    public async Task<Station> Get(int id)
    {
        var filter = Builders<Station>.Filter.Eq("Id", id);

        try
        {
            return await _context.Stations.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Station item)
    {
        try
        {
            item.Id=(int) _context.Stations.Find(_ => true).ToList().Count+1;
            await _context.Stations.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Stations.DeleteOneAsync(
                    Builders<Station>.Filter.Eq("StationId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    

    public async Task<bool> Update(string id, Station item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Stations
                                .ReplaceOneAsync(n => n.StationId.Equals(id)
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
                = await _context.Stations.DeleteManyAsync(new BsonDocument());

            return actionResult.IsAcknowledged
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Task<bool> UpdateAndroidVersion(int id, string androidV)
    {
        Station disp=getStation(id);
        disp.AndroidVersion=androidV;
        return Update(disp.StationId, disp);

    }

    public Task<bool> UpdateServicesVersion(int id, string servicesV)
    {
        Station disp=getStation(id);
        disp.ServicesVersion=servicesV;
        return Update(disp.StationId, disp);
    }

    public Task<bool> UpdateName(int id, string name)
    {
        Station disp=getStation(id);
        disp.Name=name;
        return Update(disp.StationId, disp);
    }

    public Task<bool> UpdatePosition(int id, string latitud, string longitud)
    {
        Station disp=getStation(id);
        disp.Latitude=latitud;
        disp.Longitude=longitud;
        return Update(disp.StationId, disp);        
    }

    public Station getStation(int id){
        var filter = Builders<Station>.Filter.Eq("Id", id);
        Station disp=_context.Stations.Find(filter).FirstOrDefaultAsync().Result;
        return disp;
    }


}
}