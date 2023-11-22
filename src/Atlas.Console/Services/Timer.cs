using System.Diagnostics;

namespace Atlas.Console.Services;

public class Timer : IDisposable
{
    private TextWriter writer;
    private Stopwatch stopwatch;
    private string operationName;

    public Timer(TextWriter writer, string operationName)
    {
        this.writer = writer;
        this.operationName = operationName;
        stopwatch = new Stopwatch();
        stopwatch.Start();
        writer.WriteLine($"{operationName} START");
    }

    public void Dispose()
    {
        stopwatch.Stop();
        writer.WriteLine($"{operationName} END - {stopwatch.ElapsedMilliseconds}ms elapsed");
    }
}