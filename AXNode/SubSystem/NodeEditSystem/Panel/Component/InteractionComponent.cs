﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using XLib.Base;
using XLib.Base.UIComponent;
using XLib.Base.VirtualDisk;
using XLib.Node;
using AXNode.SubSystem.NodeEditSystem.Control;
using AXNode.SubSystem.NodeEditSystem.Define;
using AXNode.SubSystem.ProjectSystem;
using AXNode.SubSystem.ResourceSystem;
using AXNode.SubSystem.WindowSystem;
using XLib.AvaloniaControl.Tool;
using AXNode.SubSystem.NodeEditSystem.Control;
using XLib.AvaloniaControl;

namespace AXNode.SubSystem.NodeEditSystem.Panel.Component
{
    /// <summary>
    /// 交互组件：处理键盘、鼠标事件
    /// </summary>
    public class InteractionComponent : Component<EditPanel>
    {
        #region 属性

        public NodeView? HoveredCard => _hoveredNodeView;

        #endregion

        #region 生命周期

        protected override void Init()
        {
            _tool = new SelectTool(this);
            _tool.Init();
            _hoverToolBar = new HoverToolBar
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            _host.LayerBox_ToolBar.Children.Add(_hoverToolBar);
            _hoverToolBar.UpdateLayout();
            _hoverToolBar.IsVisible = false;
            _hoverToolBar.Init();
            _hoverToolBar.ToolClick += HoverToolBar_ToolClick;
        }

        protected override void Enable()
        {
            _host.OperateArea.PointerMoved += OperateArea_MouseMove;
            _host.OperateArea.PointerPressed += OperateArea_MouseDown;
            _host.OperateArea.PointerReleased += OperateArea_MouseUp;
        }

        protected override void Reset()
        {
            ResetComponent();
        }

        protected override void Disable()
        {
            ResetComponent();
            _host.OperateArea.PointerMoved -= OperateArea_MouseMove;
            _host.OperateArea.PointerPressed -= OperateArea_MouseDown;
            _host.OperateArea.PointerReleased -= OperateArea_MouseUp;
            _hoverToolBar.IsVisible = false;
        }

        protected override void Remove()
        {
            ResetComponent();
            _host.OperateArea.PointerMoved -= OperateArea_MouseMove;
            _host.OperateArea.PointerPressed -= OperateArea_MouseDown;
            _host.OperateArea.PointerReleased -= OperateArea_MouseUp;
            _hoverToolBar.ToolClick -= HoverToolBar_ToolClick;
            _host.LayerBox_ToolBar.Children.Remove(_hoverToolBar);
            _hoverToolBar = null;
        }

        #endregion

        #region 公开方法

        public async void HandleKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                List<NodeView> cardList = GetComponent<CardComponent>().SelectedCardList;
                if (cardList.Count == 0) return;

                bool? delete = await WM.ShowAsk("确定删除选中节点吗？", "确定", false, TipLevel.Warning);
                if (delete != true) return;

                DeleteNode(cardList);
            }
            else if (e.Key == Key.Space)
            {
                List<NodeView> cardList = GetComponent<CardComponent>().SelectedCardList;
                if (cardList.Count == 0) return;

                foreach (NodeView card in cardList)
                {
                    if (card.NodeInstance.State == NodeState.Disable) card.NodeInstance.Start();
                    else card.NodeInstance.Stop();
                }
            }
        }

        /// <summary>
        /// 处理放下
        /// </summary>
        public void HandleDrop(List<ITreeItem> itemList)
        {
            // 获取屏幕坐标
            var screenPoint = Mouse.GetPosition(_host.OperateArea, true);
            // 获取节点组件
            var component = GetComponent<NodeComponent>();
            // 放下节点
            bool added = false;
            foreach (var item in itemList)
            {
                if (item is File file && file.Instance is NodeType nodeType)
                {
                    // 放下并创建节点卡片
                    NodeView? nodeView = component.DropNode(file.ID, nodeType, screenPoint);
                    if (nodeView == null) continue;
                    nodeView.NodeBackMouseEnter = NodeBack_MouseEnter;
                    nodeView.NodeBackMouseLeave = NodeBack_MouseLeave;
                    nodeView.PinGroupListChanged = PinGroupListChanged;
                    nodeView.NodeChanged = NodeChanged;
                    added = true;
                }
            }

            if (added) ProjectManager.Instance.Saved = false;
        }

        /// <summary>
        /// 监听节点卡片
        /// </summary>
        public void ListenNodeCard(NodeView nodeView)
        {
            nodeView.NodeBackMouseEnter = NodeBack_MouseEnter;
            nodeView.NodeBackMouseLeave = NodeBack_MouseLeave;
            nodeView.PinGroupListChanged = PinGroupListChanged;
            nodeView.NodeChanged = NodeChanged;
        }

        /// <summary>
        /// 更新全部引脚图标
        /// </summary>
        public void UpdateAllPinIcon()
        {
            foreach (var card in GetComponent<CardComponent>().AllCard) card.UpdateAllPinIcon();
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 捕获操作图层
        /// </summary>
        public void CaptureOperationLayer() => _host.OperateArea.CaptureMouse();

        /// <summary>
        /// 释放操作图层
        /// </summary>
        public void ReleaseOperationLayer() => _host.OperateArea.ReleaseMouseCapture();

        /// <summary>
        /// 获取鼠标命中区域
        /// </summary>
        public MouseHitedArea GetHitedArea()
        {
            // 如果悬停节点不为空
            if (_hoveredNodeView != null)
                return _hoveredNodeView.HoveredPin == null ? MouseHitedArea.Node : MouseHitedArea.Pin;
            // 如果悬停于连接线
            else if (GetComponent<DrawingComponent>().HoveredConnectLine != null) return MouseHitedArea.ConnectLine;
            // 返回空白区域
            return MouseHitedArea.Space;
        }

        /// <summary>
        /// 清除悬停框
        /// </summary>
        public void ClearHoverBox()
        {
            GetComponent<DrawingComponent>().HoverBox = null;
            GetComponent<DrawingComponent>().UpdateHoverBox();
        }

        /// <summary>
        /// 处理鼠标移动
        /// </summary>
        public void HandleMouseMove()
        {
            // 重置光标
            _tool.Cursor = CursorManager.Instance.Select;
            // 悬停在引脚上：切换光标
            if (_hoveredNodeView != null && _hoveredNodeView.HoveredPin != null)
                _tool.Cursor = CursorManager.Instance.Cross;
            // 设置光标
            _host.OperateArea.Cursor = _tool.Cursor;

            // 更新悬停连接线
            GetComponent<DrawingComponent>().UpdateHoveredConnectLine(Mouse.GetPosition(_host.OperateArea));
        }

        /// <summary>
        /// 移除节点焦点
        /// </summary>
        public void RemoveNodeFocus() => _host.OperateArea.Focus();

        /// <summary>
        /// 启动并执行节点
        /// </summary>
        public void StartAndExecute()
        {
            _hoveredNodeView.NodeInstance.Start();
            _hoveredNodeView.NodeInstance.Execute();
        }

        #region 选择

        public bool CurrentNodeSelected() =>
            GetComponent<CardComponent>().SelectedCardList.Contains(_hoveredNodeView);

        public void SetTop() => GetComponent<CardComponent>().SetTop(_hoveredNodeView);

        public void AddSelect()
        {
            GetComponent<CardComponent>().AddSelect(_hoveredNodeView);
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            UpdateHoverToolBar();
            UpdatePropertyPanel();
        }

        public void RemoveSelect()
        {
            GetComponent<CardComponent>().RemoveSelect(_hoveredNodeView);
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            UpdateHoverToolBar();
            UpdatePropertyPanel();
        }

        public void ClearSelect()
        {
            GetComponent<CardComponent>().ClearSelect();
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            UpdateHoverToolBar();
            UpdatePropertyPanel();
        }

        #endregion

        #region 选框

        public void BeginDrawSelectBox() => _mouseDown = Mouse.GetPosition(_host.OperateArea);

        public void CancelDrawSelectBox() => GetComponent<DrawingComponent>().ClearSelectBox();

        public void DrawSelectBox() =>
            GetComponent<DrawingComponent>().UpdateSelectBox(_mouseDown, Mouse.GetPosition(_host.OperateArea));

        public void EndDrawSelectBox()
        {
            // 清除选框
            GetComponent<DrawingComponent>().ClearSelectBox();
            // 获取选框区域与选择方式
            Rect rect = GetComponent<DrawingComponent>().GetSelectBoxRect();
            SelectType type = GetComponent<DrawingComponent>().GetSelectType();
            // 选择节点视图
            foreach (var card in GetComponent<CardComponent>().AllCard)
            {
                Rect nodeRect = card.GetHittableRect();
                switch (type)
                {
                    case SelectType.Box:
                        if (rect.Contains(nodeRect)) GetComponent<CardComponent>().AddSelect(card);
                        break;
                    case SelectType.Cross:
                        if (rect.Intersects(nodeRect)) GetComponent<CardComponent>().AddSelect(card);
                        break;
                }
            }

            // 更新选中框
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            // 更新工具栏
            UpdateHoverToolBar();
            // 更新属性面板
            UpdatePropertyPanel();
        }

        #endregion

        #region 拖动节点

        public void BeginDragNode()
        {
            _host.OperateArea.Cursor = CursorManager.Instance.SelectAndMove;
            _mouseDown = Mouse.GetPosition(_host.OperateArea);
        }

        public void CancelDragNode()
        {
            _host.OperateArea.Cursor = _tool.Cursor;
        }

        public void DragNode()
        {
            // 更新当前坐标
            _mousePoint = Mouse.GetPosition(_host.OperateArea);
            // 计算偏移
            Point offset = new Point(_mousePoint.X - _mouseDown.X, _mousePoint.Y - _mouseDown.Y);
            // offset = new (0,0) ;

            // 对齐网格
            offset = new(Math.Round(offset.X / 10) * 10, offset.Y);
            offset = new(offset.X, Math.Round(offset.Y / 10) * 10);

            // 获取世界中心
            Point center = GetComponent<DrawingComponent>().WorldCenter;
            // 设置节点偏移并更新坐标
            foreach (var card in GetComponent<CardComponent>().SelectedCardList)
            {
                card.SetOffset(new Point(offset.X, offset.Y));
                Canvas.SetLeft(card, center.X + card.Point.X - 12);
                Canvas.SetTop(card, center.Y + card.Point.Y - 1);
            }

            // 更新选中框、连接线
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            GetComponent<DrawingComponent>().UpdateConnectLine();
            // 更新悬浮工具栏
            UpdateHoverToolBar();

            ProjectManager.Instance.Saved = false;
        }

        public void EndDragNode()
        {
            _host.OperateArea.Cursor = _tool.Cursor;
            foreach (var card in GetComponent<CardComponent>().SelectedCardList) card.ApplyOffset();
        }

        #endregion

        #region 连接线

        public void BeginDrawConnectLine()
        {
            // 设置起始引脚
            _startPin = _hoveredNodeView.HoveredPin;
            // 设置鼠标坐标
            _mouseDown = Mouse.GetPosition(_host.OperateArea);
            // 获取引脚与鼠标的偏移量
            Point offset = _hoveredNodeView.GetHoveredPinOffset();
            // 计算引脚连接点坐标
            Point pinPoint = new Point(_mouseDown.X + offset.X, _mouseDown.Y + offset.Y);
            // 开始绘制连接线
            GetComponent<DrawingComponent>().BeginDrawTempConnectLine(pinPoint);
        }

        public void CancelDrawConnectLine()
        {
            GetComponent<DrawingComponent>().ClearTempLine();
        }

        public void DrawConnectLine()
        {
            // 更新鼠标坐标
            _mousePoint = Mouse.GetPosition(_host.OperateArea);
            if (_hoveredNodeView != null && _hoveredNodeView.HoveredPin != null)
            {
                // 获取引脚与鼠标的偏移量
                Point offset = _hoveredNodeView.GetHoveredPinOffset();
                // 计算引脚连接点坐标
                _mousePoint = new Point(_mousePoint.X + offset.X, _mousePoint.Y + offset.Y);
            }

            // 根据起始引脚类型更新连接线的起点或终点
            if (_startPin.Flow == PinFlow.Input)
                GetComponent<DrawingComponent>().UpdateTempLineStart(_mousePoint);
            else
                GetComponent<DrawingComponent>().UpdateTempLineEnd(_mousePoint);
        }

        public void EndDrawConnectLine()
        {
            // 清除临时连接线
            GetComponent<DrawingComponent>().ClearTempLine();
            // 悬停引脚不为空
            if (_hoveredNodeView != null && _hoveredNodeView.HoveredPin != null)
            {
                PinBase endPin = _hoveredNodeView.HoveredPin;
                // 无法连接
                if (!CanConnect(_startPin, endPin)) return;
                // 连接引脚：将结束引脚写入起始引脚
                if (_startPin.Flow == PinFlow.Input)
                {
                    // 如果是数据引脚，先移除原有的连接线
                    if (_startPin is DataPin && _startPin.SourceList.Count > 0)
                        GetComponent<DrawingComponent>().RemoveConnectLine(_startPin.SourceList[0], _startPin);
                    // 连接源与目标。数据引脚会自动断开原有连接
                    _startPin.AddSource(endPin);
                    endPin.AddTarget(_startPin);
                    // 添加连接线
                    GetComponent<DrawingComponent>().AddConnectLine(endPin, _startPin);
                }
                else
                {
                    // 如果是数据引脚，先移除原有的连接线
                    if (endPin is DataPin && endPin.SourceList.Count > 0)
                        GetComponent<DrawingComponent>().RemoveConnectLine(endPin.SourceList[0], endPin);
                    // 连接源与目标。数据引脚会自动断开原有连接
                    _startPin.AddTarget(endPin);
                    endPin.AddSource(_startPin);
                    // 添加连接线
                    GetComponent<DrawingComponent>().AddConnectLine(_startPin, endPin);
                }

                // 更新引脚图标
                UpdateAllPinIcon();

                ProjectManager.Instance.Saved = false;
            }

            _startPin = null;
        }

        #endregion

        #region 断开引脚

        public void BeginBreakPin() => _rightHitedPin = _hoveredNodeView.HoveredPin;

        public void CancelBreakPin() => _rightHitedPin = null;

        public void EndBreakPin()
        {
            // 命中输入节点，则与源断开
            if (_rightHitedPin.Flow == PinFlow.Input)
            {
                List<PinBase> sourceList = new List<PinBase>(_rightHitedPin.SourceList);
                foreach (var source in sourceList) BreakPin(source, _rightHitedPin);
            }
            // 否则与目标断开
            else
            {
                List<PinBase> targetList = new List<PinBase>(_rightHitedPin.TargetList);
                foreach (var target in targetList) BreakPin(_rightHitedPin, target);
            }

            // 更新引脚图标
            UpdateAllPinIcon();

            _rightHitedPin = null;

            ProjectManager.Instance.Saved = false;
        }

        #endregion

        #region 移除连接线

        public void RemoveConnectLine()
        {
            // 获取连接线
            Layer.VisualConnectLine? connectLine = GetComponent<DrawingComponent>().HoveredConnectLine;
            if (connectLine == null) return;
            // 断开引脚
            BreakPin(connectLine.StartPin, connectLine.EndPin);
            // 更新引脚图标
            UpdateAllPinIcon();
        }

        #endregion

        #region 拖动视口

        public void BeginDragViewport()
        {
            _host.OperateArea.Cursor = CursorManager.Instance.Move;
            _mouseDown = Mouse.GetPosition(_host.OperateArea);
        }

        public void CancelDragViewport()
        {
            _host.OperateArea.Cursor = _tool.Cursor;
        }

        public void DragViewport()
        {
            _mousePoint = Mouse.GetPosition(_host.OperateArea);
            GetComponent<DrawingComponent>()
                .DragViewport(new Point(_mousePoint.X - _mouseDown.X, _mousePoint.Y - _mouseDown.Y));
            UpdateHoverToolBar();
        }

        public void EndDragViewport()
        {
            _host.OperateArea.Cursor = _tool.Cursor;
            GetComponent<DrawingComponent>().EndDrag();
        }

        #endregion

        #endregion

        #region 节点事件

        private void NodeBack_MouseEnter(NodeView nodeView) => SwitchHoverTarget(nodeView);

        private void NodeBack_MouseLeave(NodeView nodeView) => SwitchHoverTarget(null);

        private void PinGroupListChanged()
        {
            // 更新选中框
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            // 更新连接线
            GetComponent<DrawingComponent>().UpdateConnectLine();
            // 更新引脚图标
            UpdateAllPinIcon();
        }

        private void NodeChanged()
        {
            ProjectManager.Instance.Saved = false;
        }

        #endregion

        #region 控件事件

        private void OperateArea_MouseMove(object? sender, PointerEventArgs pointerEventArgs) => _tool?.OnMouseMove();

        private void OperateArea_MouseDown(object? sender, PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Avalonia.Controls.Control);
            if (e.ClickCount == 1) _tool?.OnMouseDown(ToMouseBtn(point));
            else if (point.Properties.IsLeftButtonPressed) _tool?.OnDoubleClick();
        }

        private void OperateArea_MouseUp(object? sender, PointerReleasedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Avalonia.Controls.Control);

            _tool?.OnMouseUp(ToMouseBtn(point));
        }

        private MouseButton ToMouseBtn(PointerPoint point)
        {
            return point.Properties.PointerUpdateKind switch
            {
                PointerUpdateKind.LeftButtonPressed => MouseButton.Left,
                PointerUpdateKind.LeftButtonReleased => MouseButton.Left,
                PointerUpdateKind.MiddleButtonPressed => MouseButton.Middle,
                PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
                PointerUpdateKind.RightButtonPressed => MouseButton.Right,
                PointerUpdateKind.RightButtonReleased => MouseButton.Right,
                _ => MouseButton.None
            };
        }

        private void HoverToolBar_ToolClick(string name)
        {
            switch (name)
            {
                case "Tool_Start":
                    foreach (var card in GetComponent<CardComponent>().SelectedCardList)
                        card.NodeInstance.Start();
                    break;
                case "Tool_Stop":
                    foreach (var card in GetComponent<CardComponent>().SelectedCardList)
                        card.NodeInstance.Stop();
                    break;
                case "Tool_Delete":
                    DeleteNode(GetComponent<CardComponent>().SelectedCardList);
                    break;
            }
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 切换悬停目标
        /// </summary>
        private void SwitchHoverTarget(NodeView? target)
        {
            // 与当前目标一致，忽略
            if (_hoveredNodeView == target) return;
            // 更新目标
            _hoveredNodeView = target;
            // 当前目标为空，清空悬停框
            if (_hoveredNodeView == null)
            {
                GetComponent<DrawingComponent>().HoverBox = null;
                GetComponent<DrawingComponent>().UpdateHoverBox();
                return;
            }

            // 当前目标已选中，不绘制悬停框
            if (GetComponent<CardComponent>().SelectedCardList.Contains(_hoveredNodeView)) return;

            // 设置悬停框
            GetComponent<DrawingComponent>().HoverBox = new TargetBox
            {
                ScreenPoint = new Point(Canvas.GetLeft(_hoveredNodeView) + 9, Canvas.GetTop(_hoveredNodeView) - 2),
                Width = _hoveredNodeView.Bounds.Width - 18,
                Height = _hoveredNodeView.Bounds.Height + 4
            };
        }

        /// <summary>
        /// 更新悬停工具栏
        /// </summary>
        private void UpdateHoverToolBar()
        {
            // 选中数量
            int selectedCount = GetComponent<CardComponent>().SelectedCardList.Count;
            // 显隐工具栏
            _hoverToolBar.IsVisible = selectedCount == 0 ? false : true;

            // 无选中
            if (selectedCount == 0)
            {
            }
            // 选中一个
            else if (selectedCount == 1)
            {
                Rect cardRect = GetComponent<CardComponent>().SelectedCardList[0].GetHittableRect();
                double left = cardRect.Right - _hoverToolBar.Bounds.Width;
                double top = cardRect.Top - 10 - _hoverToolBar.Bounds.Height;
                Canvas.SetLeft(_hoverToolBar, left + 1);
                Canvas.SetTop(_hoverToolBar, top + 1);
            }
            // 选中多个
            else
            {
                List<NodeView> selectedList = GetComponent<CardComponent>().SelectedCardList;
                Rect rect = selectedList[0].GetHittableRect();
                for (int index = 1; index < selectedList.Count; index++)
                {
                    rect = rect.Union(selectedList[index].GetHittableRect());
                }

                double left = Math.Round((rect.Right - rect.Left - _hoverToolBar.Bounds.Width) / 2) + rect.Left;
                double top = rect.Top - 10 - _hoverToolBar.Bounds.Height;
                Canvas.SetLeft(_hoverToolBar, left + 1);
                Canvas.SetTop(_hoverToolBar, top + 1);
            }
        }

        /// <summary>
        /// 更新属性面板
        /// </summary>
        private void UpdatePropertyPanel()
        {
            // 选中数量
            int selectedCount = GetComponent<CardComponent>().SelectedCardList.Count;
            // 显隐属性面板
            _host.PropertyArea.IsVisible = selectedCount == 0 ? false : true;

            if (selectedCount == 0)
            {
                _host.PropertyPanel.Instance = null;
                _host.PropertyPanel.ClearPropertyBar();
            }
            else
            {
                // 获取第一个选中的节点
                NodeView firstCard = GetComponent<CardComponent>().SelectedCardList[0];
                if (firstCard.NodeInstance.PropertyList.Count == 0)
                {
                    _host.PropertyArea.IsVisible = false;
                    return;
                }

                // 加载属性条
                _host.PropertyPanel.Instance = firstCard.NodeInstance;
                _host.PropertyPanel.LoadPropertyBar();
            }
        }

        /// <summary>
        /// 判断两个引脚能否连接
        /// </summary>
        private bool CanConnect(PinBase start, PinBase end)
        {
            // 不能连接自己
            if (end == start) return false;
            // 不能处于同一节点下
            if (end.OwnerGroup.OwnerNode == start.OwnerGroup.OwnerNode) return false;
            // 流向不能一致
            if (end.Flow == start.Flow) return false;
            // 类型必须一致
            if (end.GetType() != start.GetType()) return false;
            // 已连接
            if (start.TargetList.Contains(end)) return false;

            return true;
        }

        /// <summary>
        /// 断开引脚
        /// </summary>
        private void BreakPin(PinBase source, PinBase target)
        {
            source.TargetList.Remove(target);
            target.SourceList.Remove(source);
            GetComponent<DrawingComponent>().RemoveConnectLine(source, target);
        }

        /// <summary>
        /// 删除节点
        /// </summary>
        private void DeleteNode(List<NodeView> cardList)
        {
            // 删除节点实例与卡片
            foreach (var card in cardList)
            {
                GetComponent<NodeComponent>().DeleteNode(card.NodeInstance);
                GetComponent<CardComponent>().DeleteNodeCard(card);
            }

            // 更新引脚图标
            UpdateAllPinIcon();
            // 清空选择
            GetComponent<CardComponent>().ClearSelect();
            // 更新选中框
            GetComponent<DrawingComponent>().UpdateSelectedBox();
            // 更新悬浮工具栏
            UpdateHoverToolBar();
            // 更新属性面板
            UpdatePropertyPanel();
            // 更新鼠标悬停
            HandleMouseMove();

            ProjectManager.Instance.Saved = false;
        }

        /// <summary>
        /// 重置组件
        /// </summary>
        private void ResetComponent()
        {
            ReleaseOperationLayer();
            Host.Cursor = null;
            _tool.Reset();
            _hoveredNodeView = null;

            _mouseDown = new Point();
            _mousePoint = new Point();

            _startPin = null;
            _rightHitedPin = null;

            _hoverToolBar.IsVisible = false;
            _host.PropertyPanel.Instance = null;
            _host.PropertyPanel.ClearPropertyBar();
            _host.PropertyArea.IsVisible = false;
        }

        #endregion

        #region 字段

        private SelectTool _tool;

        /// <summary>当前悬停的节点视图</summary>
        private NodeView? _hoveredNodeView = null;

        /// <summary>当前鼠标坐标</summary>
        private Point _mousePoint = new Point();

        /// <summary>鼠标按下坐标</summary>
        private Point _mouseDown = new Point();

        /// <summary>起始引脚</summary>
        private PinBase? _startPin;

        /// <summary>右键命中引脚</summary>
        private PinBase? _rightHitedPin = null;

        /// <summary>悬浮工具栏</summary>
        private HoverToolBar _hoverToolBar;

        #endregion
    }
}