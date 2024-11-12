using System;
using System.Windows;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using XLib.Node;
using AXNode.SubSystem.ResourceSystem;

namespace AXNode.SubSystem.NodeEditSystem.Control
{
    public partial class ActionPinGroupView : PinGroupViewBase
    {
        public ActionPinGroupView() => InitializeComponent();

        public ActionPinGroup? Instance { get; set; } = null;

        public override void Init()
        {
            if (Instance == null) return;
            Title_Pin.Text = Instance.ActionName;
            Instance.ActionNameChanged = (name) => Title_Pin.Text = name;
            Icon_Pin.Content = PinIconManager.Instance.ExecutePin_Null;

            PinArea.PointerEntered += PinArea_MouseEnter;
            PinArea.PointerExited += PinArea_MouseLeave;
        }

        public override Grid GetPinArea()
        {
            if (HoveredPin != null) return PinArea;
            throw new Exception("无命中引脚");
        }

        private void PinArea_MouseEnter(object? sender, PointerEventArgs pointerEventArgs)
        {
            HoveredPin = Instance.OutputPin;
        }

        private void PinArea_MouseLeave(object? sender, PointerEventArgs pointerEventArgs)
        {
            HoveredPin = null;
        }

        public override Point GetPinOffset(NodeView card, int pinIndex) =>
            PinArea.TranslatePoint(new Point(14, 8), card) ?? new Point(0, 0);

        public override void UpdatePinIcon()
        {
            if (Instance.OutputPin.TargetList.Count == 0) Icon_Pin.Content = PinIconManager.Instance.ExecutePin_Null;
            else Icon_Pin.Content = PinIconManager.Instance.ExecutePin;
        }
    }
}