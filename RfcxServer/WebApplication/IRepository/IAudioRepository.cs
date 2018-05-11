using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IAudioRepository
    {
        Task<IEnumerable<Audio>> Get();
        Task<Audio> Get(string id);
        Task Add(Audio item);
        Task<bool> Update(string id, string body);
        Task<bool> Update(string id, Audio item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}