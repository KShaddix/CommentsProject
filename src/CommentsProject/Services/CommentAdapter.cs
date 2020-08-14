using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;

using CommentsProject.Exceptions;
using CommentsProject.Entities;
using CommentsProject.Models.ViewModels;
using CommentsProject.Services.Interfaces;

namespace CommentsProject.Services
{
    public class CommentAdapter : ICommentAdapter
    {
        private string _connectionString;

        public CommentAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Comment> GetCommentById(int id)
        {
            Comment comment;

            using (var db = new SqlConnection(_connectionString))
            {
                comment = (await db.QueryAsync<Comment>($"EXEC GetCommentById {id}")).SingleOrDefault();
            }

            if (comment == null)
                throw new AdapterException(StatusCodes.Status404NotFound, "Данный комментарий не найден");

            return comment;
        }

        public async Task<Models.FilteredModels<CommentViewModel>> GetCommentsByPage(int articleId, int page, int count = 20)
        {
            var filteredModels = new Models.FilteredModels<CommentViewModel>();
            List<Models.Comment> comments;

            using (var db = new SqlConnection(_connectionString))
            {
                int allAmount = (await db.QueryAsync<int>($"EXEC GetCommentsCountForArticle {articleId}")).Single();
                filteredModels.MaxPage = (allAmount % count == 0) ? allAmount / count : allAmount / count + 1;

                if (filteredModels.MaxPage == 0)
                    throw new AdapterException(StatusCodes.Status404NotFound, "На данной странице нет комментариев");

                if (page > filteredModels.MaxPage)
                    page = filteredModels.MaxPage;

                comments = (await db.QueryAsync<Models.Comment>($"EXEC GetCommentsByPage {articleId}, {page - 1}, {count}")).ToList();
            }

            filteredModels.Page = page;

            var allComments = comments.Select(c => new CommentViewModel
            {
                Id = c.Id,
                Text = c.Text,
                UserName = c.UserName,
                ParentId = c.ParentId,
                ChildComments = new List<CommentViewModel>()
            }).ToList();

            foreach (var allComment in allComments)
            {
                if (allComment.ParentId != null)
                    allComments
                        .Single(al => al.Id == allComment.ParentId)
                        .ChildComments
                        .Add(allComment);
            }

            filteredModels.Items = allComments.Where(al => al.ParentId == null).ToList();

            return filteredModels;
        }

        public async Task<Comment> Create(Comment comment)
        {
            int id;
            string parentId = (comment.ParentId == null) ? "null" : comment.ParentId.ToString();

            using (var db = new SqlConnection(_connectionString))
            {
                id = (await db.QueryAsync<int>($"EXEC CreateComment '{comment.Text}', {comment.ArticleId}, {comment.UserId}, {parentId}")).Single();
            }

            return new Comment
            {
                Id = id,
                Text = comment.Text,
                ArticleId = comment.ArticleId,
                UserId = comment.UserId,
                ParentId = comment.ParentId
            };
        }
    }
}
