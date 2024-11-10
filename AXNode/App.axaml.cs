using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AXNode.AppTool;
using XLib.Animate;

namespace AXNode;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        // AnimationEngine.Instance.Start();
        Init();

    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }
    
    private void Init()
    {
        // 初始化应用程序代理
        AppDelegate.Init();

        // 初始化系统数据
        SystemDataDelegate.Instance.Init();
        // 启动系统服务
        SystemServiceDelegate.Instance.Start();
        // 初始化系统工具
        SystemToolDelegate.Instance.Init();
    }
}