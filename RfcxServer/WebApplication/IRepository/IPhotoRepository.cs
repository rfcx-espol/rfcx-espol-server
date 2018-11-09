using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<Photo>> Get();
        Task<Photo> Get(string id);
        Task<Photo> Get(int id);
        Task<Photo> Get(int KindId, int PhotoId);
        Task<IEnumerable<Photo>> GetByKind(int KindId);
        Task Add(Photo item);
        Task<bool> Update(int PhotoId, Photo item);
        Task<bool> Remove(int PhotoId);
        Task<bool> RemoveAll();    
    }

}