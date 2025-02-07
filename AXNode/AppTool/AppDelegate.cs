﻿using System;
using System.Windows;
using Avalonia;
using Avalonia.Threading;

namespace AXNode.AppTool
{
    /// <summary>
    /// 应用程序代理
    /// </summary>
    public class AppDelegate
    {
        public static App Main { get; set; }

        public static string AppTitle => "XNode 1.0.3 Alpha";

        public static void Init()
        {
            if (Application.Current is App app) Main = app;
        }

        public static void Invoke(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }

        public static void BeginInvoke(Action action)
        {
            Dispatcher.UIThread.Invoke(action);
        }
    }
}