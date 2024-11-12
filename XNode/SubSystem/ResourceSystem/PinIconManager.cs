using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using XNode.SubSystem.NodeEditSystem.Define;

namespace XNode.SubSystem.ResourceSystem
{
    /// <summary>
    /// 引脚图标管理器
    /// </summary>
    public class PinIconManager
    {
        #region 单例

        private PinIconManager()
        {
        }

        public static PinIconManager Instance { get; } = new PinIconManager();

        #endregion

        #region 属性

        public string Name { get; set; } = "引脚图标管理器";

        private readonly SolidColorBrush ExecuteBrush = new SolidColorBrush(Color.FromRgb(196, 126, 255));

        public Shape ExecutePinIcon
        {
            get
            {
                var pinIcon = Application.Current.MainWindow.FindResource("PinIcons.ExecutePin");
                if (pinIcon is Polygon s)
                {
                    return new Polygon { Points = s.Points, StrokeThickness = 1 };
                }

                throw new ArgumentNullException();
            }
        }

        public Shape DataPinIcon
        {
            get
            {
                var pinIcon = Application.Current.MainWindow.FindResource("PinIcons.DataPin");

                if (pinIcon is Polygon s)
                {
                    return new Polygon { Points = s.Points, StrokeThickness = 1 };
                }

                throw new ArgumentNullException();
            }
        }

        public Shape ExecutePin_Null
        {
            get
            {
                var s = ExecutePinIcon;
                s.Stroke = ExecuteBrush;
                return s;
            }
        }

        public Shape ExecutePin
        {
            get
            {
                var s = ExecutePin_Null;
                s.Fill = ExecuteBrush;
                return s;
            }
        }

        #endregion

        #region 生命周期

        public void Init()
        {
            // GenerateExecutePinIcon();
            GenerateDataPinIcon();
        }

        #endregion

        #region 公开方法

        /// <summary>
        /// 获取数据引脚图标
        /// </summary>
        public Shape GetDataPinIcon(string dataType, bool solid)
        {
            if (solid)
            {
                var s = DataPinIcon;
                s.Fill = new SolidColorBrush(_colors[dataType]);
                return s;
            }
            else
            {
                var s = DataPinIcon;
                s.Stroke = new SolidColorBrush(_colors[dataType]);
                return s;
            }
        }

        #endregion

        Dictionary<string, Color> _colors = new();

        /// <summary>
        /// 生成数据引脚图标
        /// </summary>
        private void GenerateDataPinIcon()
        {
            _colors.Add("bool", PinColorSet.Bool);
            _colors.Add("int", PinColorSet.Int);
            _colors.Add("double", PinColorSet.Double);
            _colors.Add("string", PinColorSet.String);
            _colors.Add("byte[]", PinColorSet.ByteArray);
        }
    }
}