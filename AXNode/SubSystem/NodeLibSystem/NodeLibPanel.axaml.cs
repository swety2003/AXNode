using Avalonia.Controls;
using Avalonia.Input;
using AXNode.SubSystem.NodeLibSystem;
using XLib.Base;
using XLib.Base.VirtualDisk;
using XLib.AvaloniaControl;
using AXNode.SubSystem.CacheSystem;
using AXNode.SubSystem.ResourceSystem;

namespace AXNode.SubSystem.NodeLibSystem
{
    public partial class NodeLibPanel : UserControl
    {
        public NodeLibPanel()
        {
            InitializeComponent();
        }

        public void Init()
        {
            NodePresetTree.ItemsSource = NodeLibManager.Instance.Root.Childs;
        }

        private async void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            DataObject obj = new DataObject();
            obj.Set("object", (sender as Control).DataContext);
            await DragDrop.DoDragDrop(e, obj, DragDropEffects.Copy);
        }
    }
}