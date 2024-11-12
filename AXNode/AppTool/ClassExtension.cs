using XLib.Node;
using AXNode.SubSystem.NodeEditSystem.Define;

namespace AXNode.AppTool
{
    public static class ClassExtension
    {
        /// <summary>
        /// 获取引脚路径
        /// </summary>
        public static PinPath GetPinPath(this PinBase pin)
        {
            return new PinPath
            {
                NodeVersion = pin.OwnerGroup.OwnerNode.Version,
                NodeID = pin.OwnerGroup.OwnerNode.ID,
                GroupIndex = pin.OwnerGroup.Index,
                PinIndex = pin.OwnerGroup.GetPinIndex(pin)
            };
        }
    }
}