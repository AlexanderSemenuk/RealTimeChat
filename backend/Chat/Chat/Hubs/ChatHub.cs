using System.Text.Json;
using Chat.Data;
using Chat.Models;
using Chat.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Chat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
    public Task ReceiveMessageWithSentiment(string userName, string message, string sentiment);
}

public class ChatHub : Hub<IChatClient>
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<ChatHub> _logger;
    private readonly ISentimentAnalysisService _sentimentAnalysisService;
    private readonly ChatDbContext _dbContext;

    public ChatHub(
        IDistributedCache cache,
        ILogger<ChatHub> logger, 
        ISentimentAnalysisService sentimentAnalysisService,
        ChatDbContext dbContext)
    {
        _cache = cache;
        _logger = logger;
        _sentimentAnalysisService = sentimentAnalysisService;
        _dbContext = dbContext;
    }

    public async Task JoinChat(UserConnection connection)
    {
        try
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

            var stringConnection = JsonSerializer.Serialize(connection);

            await _cache.SetStringAsync(Context.ConnectionId, stringConnection);

            await Clients
                .Group(connection.ChatRoom)
                .ReceiveMessage("Admin", $"{connection.UserName} joined");

            _logger.LogInformation($"User {connection.UserName} joined chat room {connection.ChatRoom}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining chat");
            throw;
        }
    }

    public async Task SendMessage(string message)
    {
        try
        {
            var stringConnection = await _cache.GetStringAsync(Context.ConnectionId);

            if (stringConnection is null)
            {
                _logger.LogWarning($"Connection ID {Context.ConnectionId} not found in cache");
                return;
            }

            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            if (connection is not null)
            {
                string sentiment = await _sentimentAnalysisService.AnalyzeSentiment(message);

                ChatMessage chatMessage = new ChatMessage
                {
                    UserName = connection.UserName,
                    Message = message,
                    ChatRoom = connection.ChatRoom,
                    Sentiment = sentiment,
                    Timestamp = DateTime.UtcNow
                };

                _dbContext.ChatMessages.Add(chatMessage);

                await _dbContext.SaveChangesAsync();
                
                await Clients
                    .Group(connection.ChatRoom)
                    .ReceiveMessageWithSentiment(connection.UserName, message, sentiment);

                _logger.LogInformation($"User {connection.UserName} sent message to chat room {connection.ChatRoom}");
            }
            else
            {
                _logger.LogWarning($"Connection for ID {Context.ConnectionId} is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var stringConnection = await _cache.GetStringAsync(Context.ConnectionId);

            if (stringConnection is null)
            {
                _logger.LogWarning($"Connection ID {Context.ConnectionId} not found in cache");
                return;
            }

            var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

            if (connection is not null)
            {
                await _cache.RemoveAsync(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);

                await Clients
                    .Group(connection.ChatRoom)
                    .ReceiveMessage("Admin", $"{connection.UserName} left the chat");

                _logger.LogInformation($"User {connection.UserName} left chat room {connection.ChatRoom}");
            }
            else
            {
                _logger.LogWarning($"Connection for ID {Context.ConnectionId} is null");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error on disconnected");
            throw;
        }

        await base.OnDisconnectedAsync(exception);
    }
}