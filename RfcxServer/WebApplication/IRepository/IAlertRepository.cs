using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IAlertRepository
    {
        List<Alert> Get();
        Task<IEnumerable<Alert>> GetAllAlerts();
        Task<Alert> GetAlert(string id);
        Task AddAlert(Alert item);
        Task<bool> UpdateAlert(string id, Alert item);
        Task<bool> RemoveAlert(string id);
        // Task<IEnumerable<Condition>> GetConditions(string alertId);
        // Task <Condition> getCondition(string alertId, string conditionId);
        // Task<bool> AddCondition(string id);
        // Task <bool> editCondition(string alertId, string conditionId);
        // Task <bool> deleteCondition(string alertId, string conditionId);
        Condition getConditionObject(int alertId, int conditionId);

    }
}