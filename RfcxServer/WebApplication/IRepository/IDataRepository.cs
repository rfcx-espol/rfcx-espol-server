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
        Task Add(Data item);
        Task<bool> Update(string id, Data item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}