
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;

namespace AXNode.SubSystem.NodeEditSystem.Control.PropertyBar
{
    public partial class ListSelecter : PropertyBarBase
    {
        public ListSelecter() => InitializeComponent();

        public Action<int>? SelectionChanged { get; set; } = null;

        public void LoadProperty(List<string> list, string value)
        {
            Block_Title.Text = Title;
            foreach (var item in list)
            {
                ComboBoxItem boxItem = new ComboBoxItem
                {
                    Content = item,
                };
                ToolTip.SetTip(boxItem,item);
                Box_ItemList.Items.Add(boxItem);
            }
            Box_ItemList.SelectedIndex = list.IndexOf(value);
        }

        private void MainGrid_MouseEnter(object? sender, PointerEventArgs pointerEventArgs)
        {
            Block_Title.Foreground = _hovered;
        }

        private void MainGrid_MouseLeave(object? sender, PointerEventArgs pointerEventArgs)
        {
            Block_Title.Foreground = _default;
        }

        private void Box_ItemList_SelectionChanged(object? sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            SelectionChanged?.Invoke(Box_ItemList.SelectedIndex - 1);
        }
    }
}