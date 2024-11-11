using System;
using System.Windows;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using XLib.Node;
using AXNode.SubSystem.NodeEditSystem.Define;
using AXNode.SubSystem.ResourceSystem;

namespace AXNode.SubSystem.NodeEditSystem.Control
{
    public partial class DataPinGroupView : PinGroupViewBase
    {
        #region 构造方法

        public DataPinGroupView() => InitializeComponent();

        #endregion

        #region 属性

        public DataPinGroup? Instance { get; set; } = null;

        #endregion

        #region 基类方法

        public override void Init()
        {
            if (Instance == null) return;

            Block_Name.Text = Instance.Name;
            // Block_Name.Text = Instance.Name + GetTypeName();
            Block_Name.Foreground = new SolidColorBrush(GetDataPinColor());
            Input_Value.Text = Instance.Value;
            Input_Value.IsReadOnly = !Instance.CanInput;
            // InputBoxArea.Width = new GridLength(Instance.BoxWidth);

            // 无输入引脚：隐藏引脚图标与区域
            if (Instance.InputPin == null)
            {
                Icon_LeftPin.IsVisible = false;
                LeftPinArea.IsVisible = false;
            }
            // 设置图标
            else
            {
                Icon_LeftPin.Content = GetDataPinIcon();
                LeftPinArea.PointerEntered += LeftPinArea_MouseEnter;
                LeftPinArea.PointerExited += PinArea_MouseLeave;
            }

            // 无输出引脚：隐藏引脚图标与区域
            if (Instance.OutputPin == null)
            {
                Icon_RightPin.IsVisible = false;
                RightPinArea.IsVisible = false;
            }
            // 设置图标
            else
            {
                Icon_RightPin.Content = GetDataPinIcon();
                RightPinArea.PointerEntered += RightPinArea_MouseEnter;
                RightPinArea.PointerExited += PinArea_MouseLeave;
            }

            Instance.ValueChanged += ValueChanged;
            Input_Value.TextChanged += Input_Value_TextChanged;
        }

        public override Grid GetPinArea()
        {
            if (Instance.InputPin != null && HoveredPin == Instance.InputPin) return LeftPinArea;
            if (Instance.OutputPin != null && HoveredPin == Instance.OutputPin) return RightPinArea;
            throw new Exception("无命中引脚");
        }

        public override Point GetPinOffset(NodeView card, int pinIndex)
        {
            if (pinIndex == 0) return LeftPinArea.TranslatePoint(new Point(3, 8), card) ?? throw new Exception();
            return RightPinArea.TranslatePoint(new Point(14, 8), card)?? throw new Exception();
        }

        public override void UpdatePinIcon()
        {
            if (Instance.InputPin != null)
                Icon_LeftPin.Content = GetDataPinIcon(Instance.InputPin.SourceList.Count > 0);
            if (Instance.OutputPin != null)
                Icon_RightPin.Content = GetDataPinIcon(Instance.OutputPin.TargetList.Count > 0);
        }

        #endregion

        #region 控件事件

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

        #endregion

        #region 私有方法

        private Color GetDataPinColor()
        {
            return Instance.Type switch
            {
                "int" => PinColorSet.Int,
                "double" => PinColorSet.Double,
                "string" => PinColorSet.String,
                "bool" => PinColorSet.Bool,
                "byte[]" => PinColorSet.ByteArray,
                _ => Colors.White,
            };
        }

        private Shape GetDataPinIcon(bool solid = false)
        {
            return PinIconManager.Instance.GetDataPinIcon(Instance.Type, solid);
        }

        private void ValueChanged()
        {
            Dispatcher.UIThread.Invoke(() => Input_Value.Text = Instance.Value);
        }

        private void Input_Value_TextChanged(object sender, TextChangedEventArgs e)
        {
            Instance.Value = Input_Value.Text;
        }

        #endregion
    }
}