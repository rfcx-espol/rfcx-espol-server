using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IPhotoRepository
    {
        List<Photo> Get();
        Task<Photo> Get(string id);
        Task<Photo> Get(int id);
        Task<bool> Add(Photo item);
        bool Remove(int id);
        bool Update(int PhotoId, Photo item);
        bool UpdateDescription(int id, string description);
    }

}