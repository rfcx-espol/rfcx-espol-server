using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IAlertaRepository
    {
        Task<IEnumerable<Alerta>> Get();
        Task<Alerta> Get(string id);
        Task Add(Alerta item);
        Task<bool> Update(string id, Alerta item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}