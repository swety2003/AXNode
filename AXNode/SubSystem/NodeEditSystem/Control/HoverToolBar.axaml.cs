using System;
using System.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AXNode.SubSystem.NodeEditSystem.Control
{
    public partial class HoverToolBar : UserControl
    {
        public HoverToolBar() => InitializeComponent();

        public event Action<string> ToolClick;

        public void Init()
        {
            foreach (var item in Stack_ToolBar.Children)
                if (item is Button button)
                    button.Click += Tool_Click;
        }

        private void Tool_Click(object sender, RoutedEventArgs e) => ToolClick?.Invoke(((Button)sender).Name);
    }
}