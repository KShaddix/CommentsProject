using System.Collections.Generic;
using System.Threading.Tasks;

using CommentsProject.Models;

namespace CommentsProject.Services.Interfaces
{
    public interface IArticleAdapter
    {
        Task<Article> GetArticleById(int id);
        Task<FilteredModels<Article>> GetArticlesByPage(int page, int count = 20);
    }
}
