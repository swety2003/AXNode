using System.Collections.Generic;
using System.Windows;
using Avalonia;
using Avalonia.Media;
using AXNode.SubSystem.NodeEditSystem.Define;
using XLib.Avalonia.Drawing;

namespace AXNode.SubSystem.NodeEditSystem.Panel.Layer
{
    /// <summary>
    /// 选中框图层
    /// </summary>
    public class SelectedBoxLayer : SingleBoard
    {
        public List<TargetBox> BoxList { get; set; } = new List<TargetBox>();

        protected override void OnUpdate()
        {
            foreach (var box in BoxList)
            {
                List<Point> pointList = box.GetPointList(15);
                for (int index = 0; index < pointList.Count / 2; index++)
                    _dc.DrawLine(_pen, pointList[index * 2], pointList[index * 2 + 1]);
            }
        }

        private readonly Pen _pen = new Pen(new SolidColorBrush(Color.FromRgb(237, 100, 21)), 1);
    }
}