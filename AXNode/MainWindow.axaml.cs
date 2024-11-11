using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using AXNode.AppTool;
using AXNode.Sample.Layer;
using AXNode.SubSystem.CacheSystem;
using AXNode.SubSystem.EventSystem;
using AXNode.SubSystem.ProjectSystem;
using AXNode.SubSystem.ResourceSystem;
using AXNode.SubSystem.WindowSystem;
using XLib.Avalonia.WindowDefine;
using XLib.AvaloniaControl;
using XLib.Base;

namespace AXNode;

public partial class MainWindow : Window
{
    #region 属性

    /// <summary>核心编辑器实例</summary>
    public CoreEditer Editer
    {
        get
        {
            if (_coreEditer == null) throw new Exception("核心编辑器为空");
            return _coreEditer;
        }
    }

    #endregion

    #region 构造方法

    public MainWindow() => InitializeComponent();

    #endregion

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Closing += OnClosing;
        WindowLoaded();
        AddHandler(KeyDownEvent, (sender, args) => { Keyboard.RegisterModifiers(args.KeyModifiers); },
            RoutingStrategies.Tunnel);
        AddHandler(PointerMovedEvent, (sender, args) =>
        {
            Mouse.RegisterPointerEventArgs(args);
            Mouse.RegisterDirectlyOver(args);
        }, RoutingStrategies.Tunnel);
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        e.Cancel = await ReadyClose() == true;
    }


    #region XMainWindow 方法

    protected void WindowLoaded()
    {
        // 恢复窗口状态并监听窗口状态
        RecoverWindowState();
        ListenWindowState();

        // 设置主窗口实例
        WM.Main = this;

        // 初始化工具栏
        // InitToolBar();

        // 加载核心编辑器
        LoadCoreEditer();

        // 监听系统事件
        EM.Instance.Add(EventType.Project_Changed, UpdateTitle);
    }

    protected async Task<bool> ReadyClose()
    {
        // 项目未保存
        if (!ProjectManager.Instance.Saved)
        {
            bool? result = await WM.ShowAsk("当前项目未保存，是否保存？");
            // 保存
            if (result == true)
            {
                bool saved = ProjectManager.Instance.SaveProject();
                // 确定保存，但未执行，则取消操作
                if (!saved) return false;
            }
            // 取消操作
            else if (result == null) return false;
        }

        // 关闭项目
        ProjectManager.Instance.CloseProject();

        return true;
    }

    #endregion

    #region 窗口事件

    private void XMainWindow_PreviewKeyDown(object? sender, KeyEventArgs e)
    {
        EM.Instance.Invoke(EventType.KeyDown, e.Key.ToString());
    }

    private void XMainWindow_PreviewKeyUp(object? sender, KeyEventArgs e)
    {
        EM.Instance.Invoke(EventType.KeyUp, e.Key.ToString());
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 恢复窗口状态
    /// </summary>
    private void RecoverWindowState()
    {
        WindowState = CacheManager.Instance.Cache.MainWindow.State;
        var screenFromVisual = this.Screens.ScreenFromVisual(this);
        // 居中窗口
        Bounds = new Rect((screenFromVisual.WorkingArea.Width - Width) / 2,
            (screenFromVisual.WorkingArea.Height - Height) / 2, CacheManager.Instance.Cache.MainWindow.Width,
            CacheManager.Instance.Cache.MainWindow.Height);
    }

    /// <summary>
    /// 监听窗口状态
    /// </summary>
    private void ListenWindowState()
    {
        // StateChanged += (s, e) =>
        // {
        //     if (WindowState is WindowState.Normal or WindowState.Maximized)
        //     {
        //         CacheManager.Instance.Cache.MainWindow.State = WindowState;
        //         CacheManager.Instance.UpdateCache();
        //     }
        // };
        SizeChanged += (s, e) =>
        {
            if (WindowState == WindowState.Maximized) return;
            CacheManager.Instance.Cache.MainWindow.Width = (int)Width;
            CacheManager.Instance.Cache.MainWindow.Height = (int)Height;
            CacheManager.Instance.UpdateCache();
        };
    }


    /// <summary>
    /// 工具栏.单击工具
    /// </summary>
    private void ToolBar_ToolClick(string name)
    {
        switch (name)
        {
            // 新建项目
            case "NewProject":
                ProjectManager.Instance.NewProject();
                UpdateTitle();
                break;
            // 打开项目
            case "OpenProject":
                ProjectManager.Instance.OpenProject();
                UpdateTitle();
                break;
            // 保存项目
            case "SaveProject":
                ProjectManager.Instance.SaveProject();
                UpdateTitle();
                break;
            // 另存为项目
            case "SaveAs":
                ProjectManager.Instance.SaveAsProject();
                UpdateTitle();
                break;
            // 撤销
            case "Undo":
                break;
            // 重做
            case "Redo":
                break;
            // 控制台
            case "Console":
                if (_consoleOpened)
                {
                    OSTool.CloseConsole();
                    _consoleOpened = false;
                }
                else
                {
                    OSTool.OpenConsole();
                    _consoleOpened = true;
                }

                break;
            // 清空控制台
            case "ClearConsole":
                Console.Clear();
                break;
        }
    }

    /// <summary>
    /// 加载核心编辑器
    /// </summary>
    private void LoadCoreEditer()
    {
        _coreEditer = new CoreEditer { Margin = new Thickness(0, 2, 0, 0) };
        MainGrid.Children.Add(_coreEditer);
    }

    /// <summary>
    /// 更新标题
    /// </summary>
    private void UpdateTitle()
    {
        if (ProjectManager.Instance.CurrentProject != null)
        {
            Title = ProjectManager.Instance.CurrentProject.ProjectName;
            if (!ProjectManager.Instance.Saved) Title += "*";
            Title += " - " + AppDelegate.AppTitle;
        }
        else Title = AppDelegate.AppTitle;
    }

    #endregion

    #region 字段

    /// <summary>工具栏</summary>
    // private ToolBar? _toolBar = null;

    /// <summary>核心编辑器</summary>
    private CoreEditer? _coreEditer = null;

    /// <summary>控制台已打开</summary>
    private bool _consoleOpened = false;

    #endregion


    private void MenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        ToolBar_ToolClick((sender as MenuItem).Tag.ToString());
    }
}