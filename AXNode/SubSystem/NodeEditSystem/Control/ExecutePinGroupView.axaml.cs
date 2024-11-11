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
    public partial class ExecutePinGroupView : PinGroupViewBase
    {
        public ExecutePinGroupView() => InitializeComponent();

        public ExecutePinGroup? Instance { get; set; } = null;

        public override void Init()
        {
            if (Instance == null) return;
            Icon_LeftPin.Content = PinIconManager.Instance.ExecutePin_Null;
            Block_ExecuteDesc.Text = Instance.ExecuteDesc;
            Icon_RightPin.Content = PinIconManager.Instance.ExecutePin_Null;

            LeftPinArea.PointerEntered += LeftPinArea_MouseEnter;
            RightPinArea.PointerEntered += RightPinArea_MouseEnter;
            LeftPinArea.PointerExited += PinArea_MouseLeave;
            RightPinArea.PointerExited += PinArea_MouseLeave;
        }

        public override Grid GetPinArea()
        {
            if (HoveredPin == Instance.InputPin) return LeftPinArea;
            if (HoveredPin == Instance.OutputPin) return RightPinArea;
            throw new Exception("无命中引脚");
        }

        private void LeftPinArea_MouseEnter(object? sender, PointerEventArgs pointerEventArgs)
        {
            HoveredPin = Instance.InputPin;
        }

        private void RightPinArea_MouseEnter(object? sender, PointerEventArgs pointerEventArgs)
        {
            HoveredPin = Instance.OutputPin;
        }

        private void PinArea_MouseLeave(object? sender, PointerEventArgs pointerEventArgs)
        {
            HoveredPin = null;
        }

        public override Point GetPinOffset(NodeView card, int pinIndex)
        {
            if (pinIndex == 0) return LeftPinArea.TranslatePoint(new Point(3, 8), card) ?? throw new Exception();
            return RightPinArea.TranslatePoint(new Point(14, 8), card) ?? throw new Exception();
        }

        public override void UpdatePinIcon()
        {
            if (Instance.InputPin.SourceList.Count == 0) Icon_LeftPin.Content = PinIconManager.Instance.ExecutePin_Null;
            else Icon_LeftPin.Content = PinIconManager.Instance.ExecutePin;
            if (Instance.OutputPin.TargetList.Count == 0) Icon_RightPin.Content = PinIconManager.Instance.ExecutePin_Null;
            else Icon_RightPin.Content = PinIconManager.Instance.ExecutePin;
        }
    }
}