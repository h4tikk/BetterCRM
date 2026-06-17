using BetterCRM.Core.Interfaces.Repositories;
using BetterCRM.Core.Models;
using BetterCRM.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterCRM.DataAccess.Repositories
{
    public class ChatMessageRepository : Repository<ChatMessage, ChatMessageEntity>, IChatRepository
    {
        public ChatMessageRepository(ApplicationDbContext context) : base(context) { }

        protected override ChatMessage MapToDomain(ChatMessageEntity db) =>
            DomainMapper.ToChatMessageDomain(db);

        protected override ChatMessageEntity MapToDb(ChatMessage domain, ChatMessageEntity? existing = null) =>
            DomainMapper.ToChatMessageDb(domain, existing);


        public async Task SaveMessageAsync(ChatMessage message)
        {
            var entity = MapToDb(message);
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatMessage>> GetPrivateMessagesAsync(
            Guid userId, Guid withUserId, int take, DateTime before)
        {
            return (await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Recipient)
                .Where(m =>
                    m.SentAt < before &&
                    m.ChatRoomId == null &&
                    ((m.SenderId == userId && m.RecipientId == withUserId) ||
                     (m.SenderId == withUserId && m.RecipientId == userId)))
                .OrderByDescending(m => m.SentAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync())
                .Select(MapToDomain);
        }

        public async Task<IEnumerable<ChatMessage>> GetDepartmentMessagesAsync(
            Guid departmentId, int take, DateTime before)
        {
            return (await _dbSet
                .Include(m => m.Sender)
                .Where(m => m.ChatRoomId == departmentId && m.SentAt < before)
                .OrderByDescending(m => m.SentAt)
                .Take(take)
                .AsNoTracking()
                .ToListAsync())
                .Select(MapToDomain);
        }

        public async Task MarkAsReadAsync(Guid messageId, Guid readerId) =>
            await _dbSet
                .Where(m => m.Id == messageId && m.RecipientId == readerId && !m.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(m => m.IsRead, true));

        public async Task<bool> UserBelongsToDepartmentAsync(Guid userId, Guid departmentId) =>
            await _context.Users
                .AnyAsync(u => u.Id == userId && u.DepartmentId == departmentId);

        public async Task<Guid?> GetUserDepartmentIdAsync(Guid userId) =>
            await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.DepartmentId)
                .FirstOrDefaultAsync();
    }
}
