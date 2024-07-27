using Chat.Data;
using Chat.Hubs;
using Chat.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    var connection = builder.Configuration.GetConnectionString("Redis");
    options.Configuration = connection;
});

builder.Services.AddDbContext<ChatDbContext>(
    options =>
    {
        var connection = builder.Configuration.GetConnectionString("DbConnection");
        options.UseSqlServer(connection);
        
    }
);

builder.Services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://onlinechatui.azurewebsites.net")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


builder.Services.AddSignalR()
    .AddAzureSignalR(options =>
{
    var connection = builder.Configuration.GetConnectionString("AzureSignalRConnectionString");
    options.ConnectionString = connection;
});

var app = builder.Build();

app.MapHub<ChatHub>("/chat");

app.UseCors();

app.Run();