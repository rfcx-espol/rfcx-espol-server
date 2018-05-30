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
        Task<Audio> Get(int DispositivoId, int AudioId);
        Task<IEnumerable<Audio>> GetByDispositivo(int DispositivoId);
        Task Add(Audio item);
        Task<bool> Update(int DispositivoId, int AudioId, Audio item);
        Task<bool> Remove(int DispositivoId, int AudioId);
        Task<bool> RemoveAll();
        
    }
}