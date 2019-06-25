using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IAlertRepository
    {
        List<Alert> Get();
        Alert Get(int id);
        bool Add(Alert item);
        Task<IEnumerable<Alert>> GetAllAlerts();
        Task<Alert> GetAlert(string id);
        Task AddAlert(Alert item);
        Task<bool> UpdateAlert(string id, Alert item);
        Task<bool> RemoveAlert(string id);

    }
}