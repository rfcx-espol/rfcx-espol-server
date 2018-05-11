using MongoDB.Driver;
using System.Collections.Generic;
using WebApplication.Models;
using System.Threading.Tasks;


namespace WebApplication.IRepository
{
    public interface IAlertaConfiguracionRepository
    {
        Task<IEnumerable<AlertaConfiguracion>> Get();
        Task<AlertaConfiguracion> Get(string id);
        Task Add(AlertaConfiguracion item);
        Task<bool> Update(string id, string body);
        Task<bool> Update(string id, AlertaConfiguracion item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}