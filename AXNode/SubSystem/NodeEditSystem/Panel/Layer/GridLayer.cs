﻿using System.Runtime.CompilerServices;
using System.Windows;
using Avalonia;
using Avalonia.Media;
using XLib.Avalonia.Drawing;

namespace AXNode.SubSystem.NodeEditSystem.Panel.Layer
{
    /// <summary>
    /// 网格图层
    /// </summary>
    public class GridLayer : SingleBoard
    {
        #region 属性

        public Point GridCenter { get; set; }

        public int GridLineCount { get; private set; }

        public int GridListCount { get; private set; }

        public int CellWidth => _gridWidth / _subdivideWidth;

        public int CellHeight => _gridHeight / _subdivideHeight;

        #endregion

        #region 公开方法

        public void MoveLayer(Point offset)
        {
            _moveOffset = offset;
            Update();
        }

        public void Reset()
        {
            _centerOffset = new Point();
            Update();
        }

        public override void Render(DrawingContext context)
        {
            DrawGrid(context);
        }

        /// <summary>
        /// 结束拖动时调用
        /// </summary>
        public void ApplyOffset()
        {
            _centerOffset = new Point(_centerOffset.X + _moveOffset.X, _centerOffset.Y + _moveOffset.Y);
            _moveOffset = new Point();
        }

        /// <summary>
        /// 获取世界坐标
        /// </summary>
        public Point GetWorldPoint(Point screenPoint)
        {
            return new Point(screenPoint.X - GridCenter.X, screenPoint.Y - GridCenter.Y);
        }

        /// <summary>
        /// 获取屏幕坐标
        /// </summary>
        public Point GetScreenPoint(Point worldPoint)
        {
            return new Point(GridCenter.X + worldPoint.X, GridCenter.Y + worldPoint.Y);
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 绘制网格
        /// </summary>
        /// <param name="context"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawGrid(DrawingContext context)
        {
            int line;
            int list;
            int subIndex;

            // 更新中心点
            Point realOffset = new Point(_centerOffset.X + _moveOffset.X, _centerOffset.Y + _moveOffset.Y);
            GridCenter = new Point((int)Bounds.Width / 2 + realOffset.X, (int)Bounds.Height / 2 + realOffset.Y);

            // 更新绘图起始点
            UpdateDrawStart();
            // 更新网格线数量
            UpdateGridLineCount();

            // 绘制细分线
            _currentPen = _microLine;
            for (line = 0; line < GridLineCount; line++)
            for (subIndex = 1; subIndex < _subdivideHeight; subIndex++)
                DrawHorizontalLine(context, line * _gridHeight + _gridHeight / _subdivideHeight * subIndex);
            for (list = 0; list < GridListCount; list++)
            for (subIndex = 1; subIndex < _subdivideWidth; subIndex++)
                DrawVerticalLine(context, list * _gridWidth + _gridWidth / _subdivideWidth * subIndex);

            // 绘制网格线
            _currentPen = _normalLine;
            for (line = 0; line < GridLineCount; line++)
            {
                if (_drawStart.Y + line * _gridHeight == GridCenter.Y) continue;
                DrawHorizontalLine(context, line * _gridHeight);
            }

            for (list = 0; list < GridListCount; list++)
            {
                if (_drawStart.X + list * _gridWidth == GridCenter.X) continue;
                DrawVerticalLine(context, list * _gridWidth);
            }

            // 绘制中心线
            _currentPen = _centerLine;
            DrawHorizontalLine(context, (int)GridCenter.Y - (int)_drawStart.Y);
            _currentPen = _centerList;
            DrawVerticalLine(context, (int)GridCenter.X - (int)_drawStart.X);
        }

        /// <summary>
        /// 更新绘图起始点，即左上角
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateDrawStart()
        {
            // 中心点横坐标 < 0
            if (GridCenter.X < 0)
                _drawStart = new Point(GridCenter.X + (int)(0 - GridCenter.X) / _gridWidth * _gridWidth,
                    _drawStart.Y);
            else
                _drawStart = new Point(GridCenter.X - ((int)GridCenter.X / _gridWidth + 1) * _gridWidth,
                    _drawStart.Y);

            // 中心点纵坐标 < 0
            if (GridCenter.Y < 0)
                _drawStart = new Point(_drawStart.X,
                    GridCenter.Y + (int)(0 - GridCenter.Y) / _gridHeight * _gridHeight);
            else
                _drawStart = new Point(_drawStart.X,
                    GridCenter.Y - ((int)GridCenter.Y / _gridHeight + 1) * _gridHeight);
        }

        /// <summary>
        /// 更新网格线数量
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateGridLineCount()
        {
            GridLineCount = (int)(Bounds.Height / _gridHeight) + 2;
            GridListCount = (int)(Bounds.Width / _gridWidth) + 2;
        }

        /// <summary>
        /// 绘制水平线
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawHorizontalLine(DrawingContext context, int y)
        {
            int realy = (int)_drawStart.Y + y;
            if (realy < 0 || realy > Bounds.Height) return;
            context.DrawLine(_currentPen, new Point(0, realy), new Point(Bounds.Width, realy));
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawVerticalLine(DrawingContext context, int x)
        {
            int realx = (int)_drawStart.X + x;
            if (realx < 0 || realx > Bounds.Width) return;
            context.DrawLine(_currentPen, new Point(realx, 0), new Point(realx, Bounds.Height));
        }

        #endregion

        #region 画笔、画刷

        private readonly Pen _normalLine = new(new SolidColorBrush(Color.FromArgb(255, 30, 30, 30)), 2);
        private readonly Pen _microLine = new(new SolidColorBrush(Color.FromArgb(255, 20, 20, 20)), 2);
        private readonly Pen _centerLine = new(new SolidColorBrush(Color.FromArgb(255, 60, 60, 60)), 2);
        private readonly Pen _centerList = new(new SolidColorBrush(Color.FromArgb(255, 60, 60, 60)), 2);
        private Pen? _currentPen;

        #endregion

        #region 字段

        /// <summary>格子宽度</summary>
        private readonly int _gridWidth = 120;

        /// <summary>宽度细分量</summary>
        private readonly int _subdivideWidth = 4;

        /// <summary>格子高度</summary>
        private readonly int _gridHeight = 120;

        /// <summary>高度细分量</summary>
        private readonly int _subdivideHeight = 4;

        private Point _drawStart = new Point();
        private Point _centerOffset = new Point();
        private Point _moveOffset = new Point();

        #endregion
    }
}