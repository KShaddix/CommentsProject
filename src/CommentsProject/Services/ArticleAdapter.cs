using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;

using CommentsProject.Exceptions;
using CommentsProject.Models;
using CommentsProject.Services.Interfaces;

namespace CommentsProject.Services
{
    public class ArticleAdapter : IArticleAdapter
    {
        private string _connectionString;

        public ArticleAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Article> GetArticleById(int id)
        {
            Article article;

            using (var db = new SqlConnection(_connectionString))
            {
                article = (await db.QueryAsync<Article>($"EXEC GetArticleInfoById {id}")).SingleOrDefault();
            }

            if (article == null)
                throw new AdapterException(StatusCodes.Status404NotFound, "Данная статья не найдена");

            return article;
        }

        public async Task<FilteredModels<Article>> GetArticlesByPage(int page, int count = 20)
        {
            var filteredModels = new FilteredModels<Article>();

            using (var db = new SqlConnection(_connectionString))
            {
                int allAmount = (await db.QueryAsync<int>("EXEC GetArticlesCount")).Single();
                filteredModels.MaxPage = (allAmount % count == 0) ? allAmount / count : allAmount / count + 1;

                if (filteredModels.MaxPage == 0)
                    throw new AdapterException(StatusCodes.Status404NotFound, "На данной странице нет статей");

                if (page > filteredModels.MaxPage)
                    page = filteredModels.MaxPage;

                filteredModels.Items = (await db.QueryAsync<Article>($"EXEC GetArticlesByPage {page - 1}, {count}")).ToList();
            }

            filteredModels.Page = page;

            return filteredModels;
        }
    }
}
