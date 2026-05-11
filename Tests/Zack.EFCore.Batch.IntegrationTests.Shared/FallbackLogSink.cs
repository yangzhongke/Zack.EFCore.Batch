using Microsoft.Extensions.Logging;

namespace Zack.EFCore.Batch.IntegrationTests.Shared;

public sealed class FallbackLogSink : ILoggerProvider
{
    public List<string> Messages { get; } = new();

    public ILogger CreateLogger(string categoryName)
    {
        return new SinkLogger(Messages);
    }

    public void Dispose()
    {
    }

    private sealed class SinkLogger : ILogger
    {
        private readonly List<string> _messages;

        public SinkLogger(List<string> messages)
        {
            _messages = messages;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _messages.Add(formatter(state, exception));
        }
    }
}

