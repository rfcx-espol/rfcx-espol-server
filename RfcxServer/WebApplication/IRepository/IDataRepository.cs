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
        Task<Data> Get(int DeviceId, int SensorId, int DataId);
        Task<IEnumerable<Data>> GetLasts();
        Task<IEnumerable<Data>> GetByDevice(int DeviceId);
        Task<Data> GetLastByDevice(int DeviceId);

        Task<IEnumerable<Data>> GetByDeviceSensor(int DeviceId, int SensorId);
        Task<Data> GetLastByDeviceSensor(int DeviceId, int SensorId);
        Task<IEnumerable<Data>> GetByDeviceSensorTimestamp(int DeviceId, int SensorId, long StartTimestamp, long EndTimestamp);
        Task Add(Data item);
        Task<bool> Update(string id, Data item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}