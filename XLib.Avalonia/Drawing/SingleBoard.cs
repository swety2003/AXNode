using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XLib.Avalonia.Drawing
{
    /// <summary>
    /// 简单画板。仅包含一个可视对象
    /// </summary>
    public abstract class SingleBoard : Control
    {
        public void Update()
        {
            IsVisible = true;
            if (IsEnabled)
            {
                InvalidateVisual();
            }
        }

        public void Clear()
        {
            IsVisible = false;
            InvalidateVisual();
        }

        /// <summary>
        /// 绘制顶点
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void DrawVertex(DrawingContext dc, Point point, double size, IBrush brush, Pen pen)
        {
            double x = point.X - size / 2;
            double y = point.Y - size / 2;
            dc.DrawRectangle(brush, pen, new Rect(x, y, size, size));
        }
    }
}