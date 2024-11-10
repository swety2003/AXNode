using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XLib.Avalonia.Drawing
{
    /// <summary>
    /// 画板。可绘制多个可视对象
    /// </summary>
    public abstract class DrawingBoard : Control
    {
        #region FrameworkElement 属性、方法

        // protected override int VisualChildrenCount => _elementList.Count;
        //
        // protected override Visual GetVisualChild(int index) => _elementList[index];

        #endregion

        #region 公开方法

        /// <summary>
        /// 添加可视元素
        /// </summary>
        public void AddVisualElement(VisualElement element)
        {
            _elementList.Add(element);

            VisualChildren.Add(element);
            LogicalChildren.Add(element);
        }

        /// <summary>
        /// 移除可视元素
        /// </summary>
        public void RemoveVisualElement(VisualElement element)
        {
            if (_hitedElement != null && _hitedElement == element) _hitedElement = null;
            _elementList.Remove(element);

            VisualChildren.Remove(element);
            LogicalChildren.Remove(element);
        }

        /// <summary>
        /// 清空可视元素
        /// </summary>
        public void ClearVisualElement()
        {
            foreach (var item in _elementList)
            {
                VisualChildren.Remove(item);
                LogicalChildren.Remove(item);
            }
            _hitedElement = null;
            _elementList.Clear();
        }

        /// <summary>
        /// 获取命中的可视元素
        /// </summary>
        public VisualElement? GetHitedVisualElement(Point point)
        {
            // 置空命中元素
            _hitedElement = _elementList.FirstOrDefault(x => x.HitTest(point));
            
            // // 创建一个圆形命中检测区域
            // GeometryHitTestParameters parameters = new GeometryHitTestParameters(new EllipseGeometry(point, 8, 8));
            // // 执行命中检测
            // VisualTreeHelper.HitTest(this, null, HitTestCallback, parameters);
            // 返回命中元素
            return _hitedElement;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 命中测试回调
        /// </summary>
        // private HitTestResultBehavior HitTestCallback(HitTestResult result)
        // {
        //     if (result.VisualHit is VisualElement element) _hitedElement = element;
        //     return HitTestResultBehavior.Stop;
        // }

        #endregion

        #region 字段

        /// <summary>可视元素列表</summary>
        protected List<VisualElement> _elementList = new List<VisualElement>();
        /// <summary>命中元素</summary>
        private VisualElement? _hitedElement = null;

        #endregion
    }
}