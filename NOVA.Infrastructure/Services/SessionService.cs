using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;
using NOVA.Infrastructure.Data;
using System;

namespace NOVA.Infrastructure.Services
{
    public class SessionService : ISessionService
    {
        private readonly NovaDbContext _context;
        private readonly ILogger<SessionService> _logger;

        public SessionService(NovaDbContext context, ILogger<SessionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChatSession?> GetByIdAsync(Guid sessionId)
        {
            return await _context.ChatSessions
                .Include(s => s.Messages)
                .FirstOrDefaultAsync(s => s.Id == sessionId);
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByUserAsync(Guid userId)
        {
            return await _context.ChatSessions
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.LastActive)
                .ToListAsync();
        }

        public async Task<ChatSession> CreateSessionAsync(Guid userId, string sessionName)
        {
            var newSession = new ChatSession
            {
                UserId = userId,
                SessionName = sessionName,
                CreatedAt = DateTime.UtcNow,
                LastActive = DateTime.UtcNow,
                IsActive = true
            };

            _context.ChatSessions.Add(newSession);
            await _context.SaveChangesAsync();

            // deactivate all other sessions for this user
            var otherSessions = _context.ChatSessions
                .Where(s => s.UserId == userId && s.Id != newSession.Id);
            foreach (var session in otherSessions)
                session.IsActive = false;

            await _context.SaveChangesAsync();
            return newSession;
        }

        public async Task<bool> ActivateSessionAsync(Guid sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session == null)
                return false;

            // deactivate others
            var others = _context.ChatSessions.Where(s => s.UserId == session.UserId);
            foreach (var s in others)
                s.IsActive = s.Id == sessionId;

            session.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSessionAsync(Guid sessionId)
        {
            var session = await _context.ChatSessions.FindAsync(sessionId);
            if (session == null)
                return false;

            _context.ChatSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ClearSessionMemoryAsync(Guid sessionId)
        {
            var messages = _context.ChatMessages.Where(m => m.SessionId == sessionId);
            _context.ChatMessages.RemoveRange(messages);
            await _context.SaveChangesAsync();
        }
    }
}
