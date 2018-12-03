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
        Task<Photo> Get(int SpecieId, int PhotoId);
        Task<IEnumerable<Photo>> GetBySpecie(int SpecieId);
        void Add(Photo item);
        Task<bool> Update(int PhotoId, Photo item);
        Task<bool> UpdateDescription(int PhotoId, string description);
        Task<bool> Remove(int PhotoId);
        Task<bool> RemoveAll();    
    }

}