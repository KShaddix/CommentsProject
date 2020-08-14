using System.Threading.Tasks;

using CommentsProject.Entities;

namespace CommentsProject.Services.Interfaces
{
    public interface IUserAdapter
    {
        Task<User> GetById(int id);
    }
}
