using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Api.Core.Entities;
using TodoApp.Api.Core.Interfaces;
using TodoApp.Api.Infrastructure.Data;

namespace TodoApp.Api.Infrastructure.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Todo>> GetUserTodosAsync(int userId)
        {
            return await _context.Todos
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<Todo?> GetTodoByIdAsync(int id, int userId)
        {
            return await _context.Todos
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        public async Task<Todo> CreateTodoAsync(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task UpdateTodoAsync(Todo todo)
        {
            _context.Todos.Update(todo);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTodoAsync(Todo todo)
        {
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
        }
    }
}
