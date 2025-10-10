using LiteDB;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.Infrastructure.Services
{
    public class ConversationMemoryService : IConversationMemoryService
    {
        private readonly string _dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "NOVA_Memory.db");

        public ConversationMemoryService()
        {
            var dir = Path.GetDirectoryName(_dbPath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public void AddMessage(string sessionId, string role, string content)
        {
            using var db = new LiteDatabase(_dbPath);
            var col = db.GetCollection<MemoryMessage>(sessionId);
            col.Insert(new MemoryMessage { Role = role, Content = content, Timestamp = DateTime.UtcNow });
        }

        public List<MemoryMessage> GetMessages(string sessionId)
        {
            using var db = new LiteDatabase(_dbPath);
            var col = db.GetCollection<MemoryMessage>(sessionId);
            return col.FindAll().OrderBy(m => m.Timestamp).ToList();
        }

        public void ClearSession(string sessionId)
        {
            using var db = new LiteDatabase(_dbPath);
            db.DropCollection(sessionId);
        }

        List<(string Role, string Content)> IConversationMemoryService.GetMessages(string sessionId)
        {
            throw new NotImplementedException();
        }

        public void ClearMemory(string sessionId)
        {
            throw new NotImplementedException();
        }
    }
}
