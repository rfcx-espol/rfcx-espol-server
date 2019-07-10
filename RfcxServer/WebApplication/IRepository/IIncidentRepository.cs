using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IIncidentRepository
    {
        Task<IEnumerable<Incident>> GetAllIncidents();
        Task<Incident> GetIncident(string id);
        Task AddIncident(Incident item);
        Task<bool> UpdateIncident(string id, Incident item);
        Task<bool> RemoveIncident(string id);
    }
}