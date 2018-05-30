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
        Task<Data> Get(int DispositivoId, int SensorId, int DataId);
        Task<IEnumerable<Data>> GetByDispositivo(int DispositivoId);
        Task<IEnumerable<Data>> GetByDispositivoSensor(int DispositivoId, int SensorId);
        Task Add(Data item);
        Task<bool> Update(string id, Data item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}