namespace XLib.Base;

public class HighPrecisionTimerL : IHighPrecisionTimer
{
    System.Timers.Timer timer = new System.Timers.Timer();

    public int Interval
    {
        get => (int)timer.Interval;
        set { timer.Interval = value; }
    }


    public bool IsRunning { get; }
    public event Action? Tick;

    public HighPrecisionTimerL()
    {
    }

    public void Start()
    {
        timer.Elapsed += (sender, args) => Tick?.Invoke();
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
    }


    public void Dispose()
    {
        timer.Dispose();
    }
}