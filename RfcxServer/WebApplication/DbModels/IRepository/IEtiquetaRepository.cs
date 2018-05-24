using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IEtiquetaRepository
    {
        Task<IEnumerable<Etiqueta>> Get();
        Task<Etiqueta> Get(string id);
        Task Add(Etiqueta item);
        Task<bool> Update(string id, Etiqueta item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}