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
        Task<Audio> Get(int id);
        Task<Audio> Get(int StationId, int AudioId);
        Task<IEnumerable<Audio>> GetByStation(int StationId);
        Task Add(Audio item);
        Task<bool> Update(int StationId, int AudioId, Audio item);
        Task<bool> Remove(int StationId, int AudioId);
        Task<bool> RemoveAll();
        
    }
}