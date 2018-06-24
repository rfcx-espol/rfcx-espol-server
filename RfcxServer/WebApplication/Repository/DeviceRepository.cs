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
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ObjectContext _context =null; 

        public DeviceRepository(IOptions<Settings> settings)
        {
            _context = new ObjectContext(settings);
        } 


        public async Task<IEnumerable<Device>> Get()
        {
            try
            {
                return await _context.Devices.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    public async Task<Device> Get(string apiKey)
    {
        var filter = Builders<Device>.Filter.Eq("APIKey", apiKey);

        try
        {
            return await _context.Devices.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public int GetDeviceCount(string apiKey){
        var filter = Builders<Device>.Filter.Eq("APIKey", apiKey);
        int count = _context.Devices.Find(filter).ToList().Count;
        return count;
    }

    public async Task<Device> Get(int id)
    {
        var filter = Builders<Device>.Filter.Eq("Id", id);

        try
        {
            return await _context.Devices.Find(filter).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public async Task Add(Device item)
    {
        try
        {
            item.Id=(int) _context.Devices.Find(_ => true).ToList().Count+1;
            await _context.Devices.InsertOneAsync(item);
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
            DeleteResult actionResult = await _context.Devices.DeleteOneAsync(
                    Builders<Device>.Filter.Eq("DeviceId", id));

            return actionResult.IsAcknowledged 
                && actionResult.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    

    public async Task<bool> Update(string id, Device item)
    {
        try
        {
            ReplaceOneResult actionResult 
                = await _context.Devices
                                .ReplaceOneAsync(n => n.DeviceId.Equals(id)
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
                = await _context.Devices.DeleteManyAsync(new BsonDocument());

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
        Device disp=getDevice(id);
        disp.AndroidVersion=androidV;
        return Update(disp.DeviceId, disp);

    }

    public Task<bool> UpdateServicesVersion(int id, string servicesV)
    {
        Device disp=getDevice(id);
        disp.ServicesVersion=servicesV;
        return Update(disp.DeviceId, disp);
    }

    public Task<bool> UpdateName(int id, string name)
    {
        Device disp=getDevice(id);
        disp.Name=name;
        return Update(disp.DeviceId, disp);
    }

    public Task<bool> UpdatePosition(int id, string latitud, string longitud)
    {
        Device disp=getDevice(id);
        disp.Latitude=latitud;
        disp.Longitude=longitud;
        return Update(disp.DeviceId, disp);        
    }

    public Device getDevice(int id){
        var filter = Builders<Device>.Filter.Eq("Id", id);
        Device disp=_context.Devices.Find(filter).FirstOrDefaultAsync().Result;
        return disp;
    }


}
}