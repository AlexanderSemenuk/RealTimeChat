namespace Chat.Models;

public class ChatMessage
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Message { get; set; }
    public string ChatRoom { get; set; }
    public string Sentiment { get; set; }
    
    public DateTime Timestamp { get; set; }
}