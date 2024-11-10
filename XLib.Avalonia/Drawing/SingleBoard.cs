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
        public SingleBoard()
        {
            // AddVisualChild(_visual);
            // AddLogicalChild(_visual);
        }

        public Point Point { get; set; } = new Point();

        // protected override int VisualChildrenCount => 1;
        //
        // protected override Visual GetVisualChild(int index) => _visual;

        public virtual void Init() { }

        public override void Render(DrawingContext context)
        {
            _dc = context;
            if (IsEnabled) OnUpdate();
            base.Render(context);
        }

        public void Update()
        {
            IsVisible = true;
            if (IsEnabled && _dc != null)
            {
                InvalidateVisual();
            }
        }

        public void Clear()
        {
            IsVisible = false;
            InvalidateVisual();
        }

        protected abstract void OnUpdate();

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

        // private readonly DrawingVisual _visual = new DrawingVisual();
        protected DrawingContext? _dc;
    }
}