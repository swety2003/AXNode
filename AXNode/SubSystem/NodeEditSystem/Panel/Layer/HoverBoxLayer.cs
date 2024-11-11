using System.Collections.Generic;
using System.Windows;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using AXNode.SubSystem.NodeEditSystem.Define;
using XLib.Animate;
using XLib.Avalonia.Drawing;

namespace AXNode.SubSystem.NodeEditSystem.Panel.Layer
{
    /// <summary>
    /// 悬停框图层
    /// </summary>
    public class HoverBoxLayer : SingleBoard, IMotion
    {
        /// <summary>悬停框</summary>
        public TargetBox? Box { get; set; } = null;

        public override void Render(DrawingContext context)
        {
            if (Box == null) return;

            List<Point> pointList = Box.GetPointList(15);
            for (int index = 0; index < pointList.Count / 2; index++)
                context.DrawLine(_pen, pointList[index * 2], pointList[index * 2 + 1]);
        }

        public double GetMotionProperty(string propertyName) => 0;

        public void SetMotionProperty(string propertyName, double value)
        {
            if (Box == null) return;
            switch (propertyName)
            {
                case "BoxMargin":
                    Box.BoxOffset = value;
                    break;
            }
            Dispatcher.UIThread.Invoke(Update);
        }

        private readonly Pen _pen = new Pen(Brushes.White, 1);
    }
}