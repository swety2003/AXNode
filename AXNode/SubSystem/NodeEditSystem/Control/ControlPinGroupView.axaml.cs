using System.Windows;
using AXNode.SubSystem.NodeEditSystem.Control;
using XLib.Node;

namespace AXNode.SubSystem.NodeEditSystem.Control
{
    /// <summary>
    /// 控件引脚组视图
    /// </summary>
    public partial class ControlPinGroupView : PinGroupViewBase
    {
        public ControlPinGroupView() => InitializeComponent();

        public ControlPinGroup? Instance { get; set; } = null;

        public override void Init()
        {
            if (Instance == null) return;
            ControlBox.Children.Add((Avalonia.Controls.Control)Instance.ControlInstance);
        }
    }
}