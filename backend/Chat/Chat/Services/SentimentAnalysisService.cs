using Azure.AI.TextAnalytics;
using Azure;


namespace Chat.Services;

public class SentimentAnalysisService : ISentimentAnalysisService
{
    private readonly TextAnalyticsClient _client;
    
    public SentimentAnalysisService(IConfiguration configuration)
    {
        var endpoint = configuration.GetConnectionString("AzureCognitiveServicesEndpoint");
        var apiKey = configuration.GetConnectionString("AzureCognitiveServicesApiKey");
        
        var credentials = new AzureKeyCredential(apiKey);
        
        _client = new TextAnalyticsClient(new Uri(endpoint), credentials);
    }
    
    public async Task<string> AnalyzeSentiment(string text)
    {
        var response = await _client.AnalyzeSentimentAsync(text);
        return response.Value.Sentiment.ToString();
    }
}