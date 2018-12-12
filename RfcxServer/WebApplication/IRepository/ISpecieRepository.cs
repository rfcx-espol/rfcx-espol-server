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
        Task<Specie> Get(int id);
        Task<bool> Add(Specie item);
        Task<bool> Update(int SpecieId, Specie item);
        Task<bool> AddPhoto(int specieId, Photo photo); 
    }

}