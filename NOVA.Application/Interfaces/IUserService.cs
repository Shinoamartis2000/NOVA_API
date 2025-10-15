using NOVA.Core.Models;

namespace NOVA.Application.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password);
        Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string password);
        Task<User?> GetByIdAsync(Guid userId);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(Guid userId);
    }
}
