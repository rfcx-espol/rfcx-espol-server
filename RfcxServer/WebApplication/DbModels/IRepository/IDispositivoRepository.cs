using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IDispositivoRepository
    {
        Task<IEnumerable<Dispositivo>> Get();
        Task<Dispositivo> Get(string id);
        Task Add(Dispositivo item);
        Task<bool> Update(string id, Dispositivo item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}