using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NOVA.Core.Models;
using NOVA.Infrastructure.Data;
using NOVA.Application.Interfaces;
using BCrypt.Net;

namespace NOVA.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly NovaDbContext _context;

        public UserService(NovaDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, User? User)> RegisterAsync(string username, string password)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return (false, "Username already exists.", null);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                VoiceType = "default",
                PreferencesJson = "{}",
                CreatedAt = DateTime.UtcNow,
                LastActive = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return (true, "Registration successful.", user);
        }

        public async Task<(bool Success, string Token, User? User)> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return (false, string.Empty, null);

            user.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()); // placeholder token
            return (true, token, user);
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.Include(u => u.Sessions).FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
