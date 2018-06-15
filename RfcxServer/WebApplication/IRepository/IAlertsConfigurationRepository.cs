using MongoDB.Driver;
using System.Collections.Generic;
using WebApplication.Models;
using System.Threading.Tasks;


namespace WebApplication.IRepository
{
    public interface IAlertsConfigurationRepository
    {
        Task<IEnumerable<AlertsConfiguration>> Get();
        Task<AlertsConfiguration> Get(string id);
        Task Add(AlertsConfiguration item);
        Task<bool> Update(string id, AlertsConfiguration item);
        Task<bool> Remove(string id);
        Task<bool> RemoveAll();
    }
}