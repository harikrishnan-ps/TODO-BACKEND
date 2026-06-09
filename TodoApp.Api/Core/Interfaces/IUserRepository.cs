using System.Threading.Tasks;
using TodoApp.Api.Core.Entities;

namespace TodoApp.Api.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
