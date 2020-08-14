using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Dapper;

using CommentsProject.Entities;
using CommentsProject.Exceptions;
using CommentsProject.Services.Interfaces;

namespace CommentsProject.Services
{
    public class UserAdapter : IUserAdapter
    {
        private string _connectionString;

        public UserAdapter(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<User> GetById(int id)
        {
            User user;

            using (var db = new SqlConnection(_connectionString))
            {
                user = (await db.QueryAsync<User>($"EXEC GetUser {id}")).SingleOrDefault();
            }

            if (user == null)
                throw new AdapterException(StatusCodes.Status404NotFound, "Данный пользователь не найден");

            return user;
        }
    }
}
