using Avalonia;
using Avalonia.Media;
using Avalonia.Rendering;

namespace XLib.Avalonia.Drawing
{
    /// <summary>
    /// 可视元素
    /// </summary>
    public abstract class VisualElement : Visual, ICustomHitTest
    {
        private DrawingContext? _dc;
        
        public override void Render(DrawingContext context)
        {
            _dc = context;
            OnUpdate(context);
            base.Render(context);
        }

        public void Update()
        {
            IsVisible = true;
            if (_dc == null) return;
            // OnUpdate(_dc);
            InvalidateVisual();
        }
    
        public void Clear()
        {
            IsVisible = false;
            InvalidateVisual();
        }
    
        protected abstract void OnUpdate(DrawingContext context);
        
        public virtual bool HitTest(Point point)
        {
            return false;
        }
    }
}