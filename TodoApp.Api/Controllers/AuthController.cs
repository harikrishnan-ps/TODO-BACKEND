using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApp.Api.Application.DTOs;
using TodoApp.Api.Application.Services;

namespace TodoApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                var response = await _authService.RegisterAsync(dto);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while registering the user." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while logging in." });
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var name = User.FindFirstValue(ClaimTypes.Name);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new { name, email });
        }
    }
}
