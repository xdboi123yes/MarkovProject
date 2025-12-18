// MarkovWebApp/Hubs/GeneratorHub.cs

using Microsoft.AspNetCore.SignalR;
using MarkovWebApp.Logic;
using System.Text;

namespace MarkovWebApp.Hubs
{
    public class GeneratorHub : Hub
    {
        private readonly MarkovGenerator _generator;
        private readonly ILogger<GeneratorHub> _logger;

        public GeneratorHub(MarkovGenerator generator, ILogger<GeneratorHub> logger)
        {
            _generator = generator;
            _logger = logger;
        }

        public async Task GenerateStream(string seedPhrase, int length = 100)
        {
            try
            {
                var postProcessor = new TextPostProcessor();
                
                _logger.LogInformation("Streaming started for seed phrase: {SeedPhrase}", seedPhrase);

                await foreach (var word in _generator.GenerateTextAsync(seedPhrase, length))
                {
                    string cleanedWord = postProcessor.ProcessNextWord(word);
                    if (!string.IsNullOrEmpty(cleanedWord))
                    {
                        await Clients.Caller.SendAsync("ReceiveWord", cleanedWord);
                    }
                }

                string? finalFlush = postProcessor.Flush();
                if (!string.IsNullOrEmpty(finalFlush))
                {
                    await Clients.Caller.SendAsync("ReceiveWord", finalFlush);
                }

                await Clients.Caller.SendAsync("StreamFinished");
                _logger.LogInformation("Streaming finished successfully for seed phrase: {SeedPhrase}", seedPhrase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred in GenerateStream for seed phrase {SeedPhrase}", seedPhrase);
                
                await Clients.Caller.SendAsync("StreamError", "An unexpected error occurred on the server.");
                
                throw;
            }
        }
    }

    public class TextPostProcessor
    {
        private bool _isInsideQuote = false;
        private string? _lastToken = null;

        public string ProcessNextWord(string word)
        {
            bool addSpace = true;

            if (_lastToken == null)
            {
                addSpace = false;
            }
            else if (word.Length == 1 && ".?!,;'".Contains(word))
            {
                addSpace = false;
            }
            else if (_lastToken == "\"" && _isInsideQuote)
            {
                addSpace = false;
            }
            else if (_lastToken == "\"" && _isInsideQuote)
            {
                addSpace = false;
            }
            else if (_lastToken == "\"" && _isInsideQuote)
            {
                addSpace = false;
            }

            string output = addSpace ? " " + word : word;

            if (word == "\"")
            {
                _isInsideQuote = !_isInsideQuote;
            }
            _lastToken = word;

            return output;
            if (word == "\"")
            {
                _isInsideQuote = !_isInsideQuote;
            }
            _lastToken = word;

            return output;
        }

        public string? Flush()
        {
            if (_isInsideQuote)
            {
                _isInsideQuote = false;
                return "\"";
            }
            return null;
        }
    }


}