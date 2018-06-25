using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IDataRepository
    {
        Task<IEnumerable<Data>> Get();
        Task<Data> Get(string id);
        Task<Data> Get(int id);
        Task<Data> Get(int StationId, int SensorId, int DataId);
        Task<IEnumerable<Data>> GetLasts();
        Task<IEnumerable<Data>> GetByStation(int StationId);
        Task<Data> GetLastByStation(int StationId);

        Task<IEnumerable<Data>> GetByStationSensor(int StationId, int SensorId);
        Task<Data> GetLastByStationSensor(int StationId, int SensorId);
        Task<IEnumerable<Data>> GetByStationSensorTimestamp(int StationId, int SensorId, long StartTimestamp, long EndTimestamp);
        Task Add(Data item);
        Task<bool> Update(string id, Data item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}