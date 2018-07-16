using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IAlertRepository
    {
        Task<IEnumerable<Alert>> Get();
        Task<Alert> Get(string id);
        Task Add(Alert item);
        Task<bool> Update(string id, Alert item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
        Task<bool> UpdateLastNotification(int id, string lastNotification);
    }
}