
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AXNode.SubSystem.NodeEditSystem.Define;

namespace AXNode.SubSystem.ResourceSystem
{
    /// <summary>
    /// 引脚图标管理器
    /// </summary>
    public class PinIconManager
    {
        #region 单例

        private PinIconManager() { }
        public static PinIconManager Instance { get; } = new PinIconManager();

        #endregion

        #region 属性

        public string Name { get; set; } = "引脚图标管理器";

        public Bitmap ExecutePin_Null => _executePin_Null;

        public Bitmap ExecutePin => _executePin;

        #endregion

        #region 生命周期

        public void Init()
        {
            GenerateExecutePinIcon();
            GenerateDataPinIcon();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取数据引脚图标
        /// </summary>
        public Bitmap GetDataPinIcon(string dataType, bool solid)
        {
            if (solid) return _dataPinIconDict[dataType];
            return _dataPinIconDict[$"{dataType}_null"];
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 生成执行引脚图标
        /// </summary>
        private void GenerateExecutePinIcon()
        {
            byte[] iconData = PinIconTool.CreatePinIcon(PinType.Execute, 196, 126, 255);
            _executePin_Null = CreateSource(11, iconData);
            iconData = PinIconTool.CreatePinIcon(PinType.Execute, 196, 126, 255, PinStyle.Solid);
            _executePin = CreateSource(11, iconData);
        }

        /// <summary>
        /// 生成数据引脚图标
        /// </summary>
        private void GenerateDataPinIcon()
        {
            GenerateDataPinIcon("bool_null", PinColorSet.Bool);
            GenerateDataPinIcon("int_null", PinColorSet.Int);
            GenerateDataPinIcon("double_null", PinColorSet.Double);
            GenerateDataPinIcon("string_null", PinColorSet.String);
            GenerateDataPinIcon("byte[]_null", PinColorSet.ByteArray);

            GenerateDataPinIcon("bool", PinColorSet.Bool, PinStyle.Solid);
            GenerateDataPinIcon("int", PinColorSet.Int, PinStyle.Solid);
            GenerateDataPinIcon("double", PinColorSet.Double, PinStyle.Solid);
            GenerateDataPinIcon("string", PinColorSet.String, PinStyle.Solid);
            GenerateDataPinIcon("byte[]", PinColorSet.ByteArray, PinStyle.Solid);
        }

        /// <summary>
        /// 生成数据引脚图标
        /// </summary>
        private void GenerateDataPinIcon(string key, Color color, PinStyle style = PinStyle.Hollow)
        {
            byte[] iconData = PinIconTool.CreatePinIcon(PinType.Data, color.R, color.G, color.B, style);
            _dataPinIconDict.Add(key, CreateSource(11, iconData));
        }

        /// <summary>
        /// 创建图源
        /// </summary>
        // private Bitmap CreateSource(int size, byte[] iconData) =>
        //     new Bitmap(size, size, 96, 96, PixelFormats.Bgr32, null, iconData, size * 4);        
        private Bitmap CreateSource(int size, byte[] iconData) => 
            new Bitmap(PixelFormat.Bgra8888,
                AlphaFormat.Premul,
                System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(iconData, 0),
                new PixelSize(size,size),
                new Vector(96,96) ,size*4);

        #endregion

        #region 字段

        private Bitmap _executePin_Null;
        private Bitmap _executePin;

        /// <summary>数据引脚图标字典</summary>
        public Dictionary<string, Bitmap> _dataPinIconDict = new Dictionary<string, Bitmap>();

        #endregion
    }
}