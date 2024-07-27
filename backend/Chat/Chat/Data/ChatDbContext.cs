using Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<ChatMessage> ChatMessages { get; set; }
}