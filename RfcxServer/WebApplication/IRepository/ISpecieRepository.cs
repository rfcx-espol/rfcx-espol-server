using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface ISpecieRepository
    {
        Task<IEnumerable<Specie>> Get();
        Task<Specie> Get(string id);
        Task<Specie> Get(int id);
        Task<string> GetPhoto(int specieId, int photoId);
        Task<bool> Add(Specie item);
        Task<bool> Update(int SpecieId, Specie item);
        Task<bool> UpdateName(int id, string name);
        Task<bool> UpdateFamily(int id, string family);
        Task<bool> AddPhoto(int specieId, Photo photo);
        Task<bool> Remove(int SpecieId);
        Task<bool> RemoveAll();    
    }

}