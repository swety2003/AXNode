using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Rendering;

namespace XLib.Avalonia.Drawing
{
    /// <summary>
    /// 可视元素
    /// </summary>
    public abstract class VisualElement : Visual, ICustomHitTest
    {
        public void Update()
        {
            IsVisible = true;
            InvalidateVisual();
        }

        public void Clear()
        {
            IsVisible = false;
            InvalidateVisual();
        }

        bool isHitted = false;

        public virtual bool HitTest(Point point)
        {
            return isHitted;
        }
    }
}