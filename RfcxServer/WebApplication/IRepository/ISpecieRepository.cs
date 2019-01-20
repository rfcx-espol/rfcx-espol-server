using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface ISpecieRepository
    {
        List<Specie> Get();
        Task<Specie> Get(string id);
        Specie Get(int id);
        Task<Specie> GetSpecie(string name);
        Task<bool> Add(Specie item);
        bool Update(int SpecieId, Specie item);
        bool UpdateName(int id, string name);
        bool UpdateFamily(int id, string family);
        bool UpdateGallery(int id, List<Photo> gallery);
        bool UpdatePhoto(int id, int id_photo, string description);
        bool AddPhoto(int specieId, Photo photo); 
        bool Remove(int id);
    }

}