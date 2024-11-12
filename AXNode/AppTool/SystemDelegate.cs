using XLib.Animate;
using XLib.Base.AppFrame;
using AXNode.SubSystem.CacheSystem;
using AXNode.SubSystem.ControlSystem;
using AXNode.SubSystem.EventSystem;
using AXNode.SubSystem.NodeLibSystem;
using AXNode.SubSystem.OptionSystem;
using AXNode.SubSystem.ResourceSystem;
using AXNode.SubSystem.TimerSystem;

namespace AXNode.AppTool
{
    /// <summary>
    /// 系统数据代理
    /// </summary>
    public class SystemDataDelegate : ManagerDelegate
    {
        private SystemDataDelegate()
        {
            // 选项、缓存
            ManagerList.Add(OptionManager.Instance);
            ManagerList.Add(CacheManager.Instance);
            // 资源、节点库
            ManagerList.Add(ResourceManager.Instance);
            ManagerList.Add(NodeLibManager.Instance);
        }

        public static SystemDataDelegate Instance { get; } = new SystemDataDelegate();
    }

    /// <summary>
    /// 系统服务代理
    /// </summary>
    public class SystemServiceDelegate : ServiceDelegate
    {
        private SystemServiceDelegate()
        {
            // 时间引擎
            ServiceList.Add(TimeEngine.Instance);
            // 应用定时器
            ServiceList.Add(AppTimer.Instance);
            // 控制引擎
            ServiceList.Add(ControlEngine.Instance);
            // 动画引擎
            ServiceList.Add(AnimationEngine.Instance);
        }

        public static SystemServiceDelegate Instance { get; } = new SystemServiceDelegate();
    }

    /// <summary>
    /// 系统工具代理
    /// </summary>
    public class SystemToolDelegate : ManagerDelegate
    {
        private SystemToolDelegate()
        {
            // 事件管理器
            ManagerList.Add(EM.Instance);
        }

        public static SystemToolDelegate Instance { get; } = new SystemToolDelegate();
    }
}