using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IKindRepository
    {
        Task<IEnumerable<Kind>> Get();
        Task<Kind> Get(string id);
        Task<Kind> Get(int id);
        Task Add(Kind item);
        Task<bool> Update(int KindId, Kind item);
        Task<bool> Remove(int KindId);
        Task<bool> RemoveAll();    
    }

}