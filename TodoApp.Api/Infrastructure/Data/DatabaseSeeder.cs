using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TodoApp.Api.Core.Entities;
using TodoApp.Api.Infrastructure.Data;

namespace TodoApp.Api.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(TodoDbContext context)
        {
            await context.Database.MigrateAsync();

            if (!await context.Users.AnyAsync())
            {
                var hasher = new PasswordHasher<User>();
                var demoUser = new User
                {
                    Name = "Demo User",
                    Email = "demo@test.com",
                    CreatedAt = DateTime.UtcNow
                };
                
                demoUser.PasswordHash = hasher.HashPassword(demoUser, "Demo@123");

                context.Users.Add(demoUser);
                await context.SaveChangesAsync();
            }
        }
    }
}
