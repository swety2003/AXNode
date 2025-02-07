﻿using System.Windows;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using XLib.Avalonia.WindowDefine;

namespace AXNode.SubSystem.WindowSystem
{
    /// <summary>
    /// 提示级别
    /// </summary>
    public enum TipLevel
    {
        /// <summary>信息</summary>
        Info,

        /// <summary>中断</summary>
        Break,

        /// <summary>警告</summary>
        Warning,

        /// <summary>错误</summary>
        Error,
    }

    /// <summary>
    /// 询问结果
    /// </summary>
    public enum AskResult
    {
        Yes,
        No,
        Cancel,
    }

    public partial class AskDialog : XDialog
    {
        #region 构造方法

        public AskDialog()
        {
            InitializeComponent();
            KeyDown += AskDialog_KeyDown;
            Yes.Click += Yes_Click;
            No.Click += No_Click;
        }

        #endregion

        #region 属性

        /// <summary>提示级别</summary>
        public TipLevel Level { get; set; } = TipLevel.Info;

        /// <summary>提示信息</summary>
        public string Message { get; set; } = "";

        /// <summary>“是”按钮文本</summary>
        public string YesText { get; set; } = "是";

        /// <summary>使用取消</summary>
        public bool UseCancel { get; set; } = true;

        /// <summary>询问结果</summary>
        public AskResult Result { get; set; } = AskResult.Cancel;

        #endregion

        #region 窗口事件

        protected override void XWindowLoaded()
        {
            // // 获取标题栏
            // Grid? titleBar = GetTemplateChild("TitleBar") as Grid;
            // titleBar.MouseLeftButtonDown += TitleBar_MouseLeftButtonDown; ;
            // // 获取边框对象
            // Border? titleBorder = GetTemplateChild("TitleBorder") as Border;
            // Border? clientBorder = GetTemplateChild("ClientBorder") as Border;
            // // 设置提示图标
            Icon_Info.IsVisible = false;
            Icon_Warning.IsVisible = false;
            Icon_Error.IsVisible = false;
            switch (Level)
            {
                case TipLevel.Info:
                    Icon_Info.IsVisible = true;
                    _currentBrush = _info;
                    break;
                case TipLevel.Break:
                    Title = "中断";
                    Icon_Break.IsVisible = true;
                    _currentBrush = _break;
                    break;
                case TipLevel.Warning:
                    Title = "警告";
                    Icon_Warning.IsVisible = true;
                    _currentBrush = _warning;
                    break;
                case TipLevel.Error:
                    Title = "错误";
                    Icon_Error.IsVisible = true;
                    _currentBrush = _error;
                    break;
            }

            // // 设置边框颜色
            // titleBorder.BorderBrush = _currentBrush;
            // clientBorder.BorderBrush = _currentBrush;
            // // 设置提示文本
            TipTextBlock.Inlines.Add(new Run(Message) { Foreground = _tip });
            if (!UseCancel) Cancel.IsVisible = false;
            // // 设置“是”按钮文本
            Yes.Content = YesText + "(_Y)";
        }

        private void AskDialog_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Close();
        }

        #endregion

        #region 控件事件

        // private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        private void Yes_Click(object? sender, RoutedEventArgs routedEventArgs)
        {
            Result = AskResult.Yes;
            Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = AskResult.No;
            Close();
        }

        #endregion

        #region 字段

        private readonly SolidColorBrush _tip = new SolidColorBrush(Color.FromRgb(160, 160, 160));

        private readonly SolidColorBrush _info = new SolidColorBrush(Color.FromRgb(119, 180, 255));
        private readonly SolidColorBrush _break = new SolidColorBrush(Color.FromRgb(249, 202, 124));
        private readonly SolidColorBrush _warning = new SolidColorBrush(Color.FromRgb(255, 180, 0));
        private readonly SolidColorBrush _error = new SolidColorBrush(Color.FromRgb(224, 67, 67));

        private SolidColorBrush _currentBrush;

        #endregion
    }
}