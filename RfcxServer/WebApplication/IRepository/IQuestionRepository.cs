using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.IRepository
{
    public interface IQuestionRepository
    {
        List<Question> Get();
        Task<Question> Get(string id);
        Question Get(int id);
        bool Add(Question item);
        bool Update(int id, Question item);
        bool UpdateSpecieId(int id, int specie_id);
        bool UpdateText(int id, string text);
        bool UpdateOption(int id, int index, string option);
        bool UpdateAnswer(int id, int answer);
        bool UpdateFeedback(int id, string feedback);
        bool UpdateCategory(int id, string category);
        bool UpdateStations(int id, List<Station> stations);
        bool Remove(int id);
    }

}