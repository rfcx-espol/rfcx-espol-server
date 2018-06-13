using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IDeviceRepository
    {
        Task<IEnumerable<Device>> Get();
        Task<Device> Get(string id);
        Task<Device> Get(int id);
        Task Add(Device item);
        Task<bool> Update(string id, Device item);
        Task<bool> UpdateAndroidVersion(int id, string androidV);
        Task<bool> UpdateServicesVersion(int id, string servicesV);
        Task<bool> UpdateName(int id, string name);
        Task<bool> UpdatePosition(int id, string latitud, string longitud);

        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}