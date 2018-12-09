using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IQuestionRepository
    {
        Task<IEnumerable<Question>> Get();
        Task<Question> Get(string id);
        Task<Question> Get(int id);
        Task<bool> Add(Question item);
    }

}