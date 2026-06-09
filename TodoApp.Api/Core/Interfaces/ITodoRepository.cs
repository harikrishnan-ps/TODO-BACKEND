using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApp.Api.Core.Entities;

namespace TodoApp.Api.Core.Interfaces
{
    public interface ITodoRepository
    {
        Task<IEnumerable<Todo>> GetUserTodosAsync(int userId);
        Task<Todo?> GetTodoByIdAsync(int id, int userId);
        Task<Todo> CreateTodoAsync(Todo todo);
        Task UpdateTodoAsync(Todo todo);
        Task DeleteTodoAsync(Todo todo);
    }
}
