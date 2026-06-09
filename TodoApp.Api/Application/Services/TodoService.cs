using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Api.Application.DTOs;
using TodoApp.Api.Core.Entities;
using TodoApp.Api.Core.Interfaces;

namespace TodoApp.Api.Application.Services
{
    public class TodoService
    {
        private readonly ITodoRepository _todoRepository;

        public TodoService(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<IEnumerable<TodoDto>> GetUserTodosAsync(int userId)
        {
            var todos = await _todoRepository.GetUserTodosAsync(userId);
            return todos.Select(MapToDto);
        }

        public async Task<TodoDto?> GetTodoAsync(int id, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id, userId);
            return todo != null ? MapToDto(todo) : null;
        }

        public async Task<TodoDto> CreateTodoAsync(CreateTodoDto dto, int userId)
        {
            var todo = new Todo
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = false,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _todoRepository.CreateTodoAsync(todo);
            return MapToDto(created);
        }

        public async Task<TodoDto?> UpdateTodoAsync(int id, UpdateTodoDto dto, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id, userId);
            if (todo == null) return null;

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.UpdatedAt = DateTime.UtcNow;

            await _todoRepository.UpdateTodoAsync(todo);
            return MapToDto(todo);
        }

        public async Task<bool> CompleteTodoAsync(int id, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id, userId);
            if (todo == null) return false;

            todo.IsCompleted = true;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateTodoAsync(todo);
            return true;
        }

        public async Task<bool> PendingTodoAsync(int id, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id, userId);
            if (todo == null) return false;

            todo.IsCompleted = false;
            todo.UpdatedAt = DateTime.UtcNow;
            await _todoRepository.UpdateTodoAsync(todo);
            return true;
        }

        public async Task<bool> DeleteTodoAsync(int id, int userId)
        {
            var todo = await _todoRepository.GetTodoByIdAsync(id, userId);
            if (todo == null) return false;

            await _todoRepository.DeleteTodoAsync(todo);
            return true;
        }

        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = DateTime.SpecifyKind(todo.CreatedAt, DateTimeKind.Utc),
                UpdatedAt = DateTime.SpecifyKind(todo.UpdatedAt, DateTimeKind.Utc)
            };
        }
    }
}
