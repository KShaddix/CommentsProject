using System.Threading.Tasks;

using CommentsProject.Entities;
using CommentsProject.Models.ViewModels;

namespace CommentsProject.Services.Interfaces
{
    public interface ICommentAdapter
    {
        Task<Comment> GetCommentById(int id);
        Task<Models.FilteredModels<CommentViewModel>> GetCommentsByPage(int articleId, int page, int count = 20);
        Task<Comment> Create(Comment comment);
    }
}
