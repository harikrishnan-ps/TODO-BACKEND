using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TodoApp.Api.Infrastructure.Data
{
    public class TodoDbContextFactory : IDesignTimeDbContextFactory<TodoDbContext>
    {
        public TodoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TodoDbContext>();
            var connectionString = "Server=localhost;Port=3306;Database=todo_db;User=todo_user;Password=todo_password;";
            optionsBuilder.UseMySql(connectionString, ServerVersion.Parse("8.0.0-mysql"));

            return new TodoDbContext(optionsBuilder.Options);
        }
    }
}
