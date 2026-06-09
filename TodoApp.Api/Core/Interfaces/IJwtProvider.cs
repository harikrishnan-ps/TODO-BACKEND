using TodoApp.Api.Core.Entities;

namespace TodoApp.Api.Core.Interfaces
{
    public interface IJwtProvider
    {
        string Generate(User user);
    }
}
