namespace XLib.Base;

public interface IHighPrecisionTimer : IDisposable
{
    int Interval { get; set; }
    bool IsRunning { get; }
    event Action? Tick;
    void Start();
    void Stop();
    void Dispose();
}