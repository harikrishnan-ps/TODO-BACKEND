using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Core.Entities;

namespace TodoApp.Api.Infrastructure.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Todo> Todos { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Todos)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
