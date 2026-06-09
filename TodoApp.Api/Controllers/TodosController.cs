using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApp.Api.Application.DTOs;
using TodoApp.Api.Application.Services;

namespace TodoApp.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly TodoService _todoService;

        public TodosController(TodoService todoService)
        {
            _todoService = todoService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            var todos = await _todoService.GetUserTodosAsync(userId);
            return Ok(todos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();
            var todo = await _todoService.GetTodoAsync(id, userId);
            if (todo == null) return NotFound();

            return Ok(todo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
        {
            var userId = GetUserId();
            var todo = await _todoService.CreateTodoAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
        {
            var userId = GetUserId();
            var updated = await _todoService.UpdateTodoAsync(id, dto, userId);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = GetUserId();
            var result = await _todoService.CompleteTodoAsync(id, userId);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/pending")]
        public async Task<IActionResult> Pending(int id)
        {
            var userId = GetUserId();
            var result = await _todoService.PendingTodoAsync(id, userId);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var result = await _todoService.DeleteTodoAsync(id, userId);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
