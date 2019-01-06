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
        void Add(Photo item);
    }

}