using System.Threading.Tasks;

namespace AIChatBot.Services
{
    public interface IAIService
    {
        Task<string> GetResponseAsync(string userInput);
    }
}
