using System.Windows;
using Avalonia.Threading;
using XLib.Base;
using AXNode.SubSystem.EventSystem;

namespace AXNode.SubSystem.EventSystem
{
    public class EM : EM<EventType>
    {
        #region 单例

        private EM() { }
        public static EM Instance { get; } = new EM();

        #endregion

        #region 引发事件

        public void Invoke(EventType eventType)
        {
            InnerInvoke(eventType);
        }

        public void Invoke<T1>(EventType eventType, T1 value1)
        {
            InnerInvoke(eventType, value1);
        }

        public void Invoke<T1, T2>(EventType eventType, T1 value1, T2 value2)
        {
            InnerInvoke(eventType, value1, value2);
        }

        public void Invoke<T1, T2, T3>(EventType eventType, T1 value1, T2 value2, T3 value3)
        {
            InnerInvoke(eventType, value1, value2, value3);
        }

        #endregion

        #region 引发事件

        public void BeginInvoke(EventType eventType)
        {
            Dispatcher.UIThread.Invoke(() => InnerInvoke(eventType));
        }

        public void BeginInvoke<T1>(EventType eventType, T1 value1)
        {
            Dispatcher.UIThread.Invoke(() => InnerInvoke(eventType, value1));
        }

        public void BeginInvoke<T1, T2>(EventType eventType, T1 value1, T2 value2)
        {
            Dispatcher.UIThread.Invoke(() => InnerInvoke(eventType, value1, value2));
        }

        public void BeginInvoke<T1, T2, T3>(EventType eventType, T1 value1, T2 value2, T3 value3)
        {
            Dispatcher.UIThread.Invoke(() => InnerInvoke(eventType, value1, value2, value3));
        }

        #endregion
    }
}