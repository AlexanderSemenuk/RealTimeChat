namespace Chat.Services;

public interface ISentimentAnalysisService
{ 
    Task<string> AnalyzeSentiment(string text);
}