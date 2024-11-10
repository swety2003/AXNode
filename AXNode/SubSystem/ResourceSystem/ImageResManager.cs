using System;
using System.Collections.Generic;
using System.IO;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AXNode.SubSystem.ResourceSystem
{
    public class ImageResManager
    {
        #region 单例

        private ImageResManager() { }
        public static ImageResManager Instance { get; } = new ImageResManager();

        #endregion

        /// <summary>
        /// 获取图片
        /// </summary>
        public Bitmap GetImage(string path)
        {
            if (!path.StartsWith("avares:") && !File.Exists(path))
                throw new Exception("图片不存在");

            // 已加载过此图片，直接返回
            if (_imageResDict.ContainsKey(path))
                return _imageResDict[path];

            // 创建图片实例
            var uri = new Uri($"{path}");
            var image = new Bitmap(AssetLoader.Open(uri));
            // 保存图片引用
            _imageResDict.Add(path, image);

            // 返回图片实例
            return image;
        }

        /// <summary>
        /// 获取资源图片
        /// </summary>
        public Bitmap GetAssetsImage(string path)
        {
            if (path == "") throw new Exception("路径不能为空");
            return GetImage($"avares://AXNode/Assets/{path}");
        }

        /// <summary>
        /// 获取节点图标
        /// </summary>
        public Bitmap GetNodeIcon(string iconName)
        {
            if (iconName == "") throw new Exception("图标名不能为空");
            return GetImage($"avares://AXNode/Assets/Icon16/Node/{iconName}.png");
        }

        /// <summary>
        /// 获取小图标
        /// </summary>
        public Bitmap? GetIcon15(string path)
        {
            if (path == "") throw new Exception("路径不能为空");
            return GetImage($"avares://AXNode/Assets/Icon15/{path}");
        }

        /// <summary>
        /// 获取图片字体
        /// </summary>
        public Bitmap GetImageFont(string path)
        {
            if (path == "") throw new Exception("路径不能为空");
            return GetAssetsImage($"Font/Number/{path}");
        }

        /// <summary>
        /// 获取子系统图片
        /// </summary>
        public Bitmap? GetSubSystemImage(string subSystem, string imageName)
        {
            if (subSystem == "" || imageName == "") return null;
            return GetImage($"avares://AXNode/SubSystem/{subSystem}/Image/{imageName}.png");
        }

        /// <summary>
        /// 获取子系统图片
        /// </summary>
        public Bitmap? GetSubSystemImage(string subSystem, string subPath, string imageName)
        {
            if (subSystem == "" || subPath == "" || imageName == "") return null;
            return GetImage($"avares://AXNode/SubSystem/{subSystem}/{subPath}/{imageName}.png");
        }

        private readonly Dictionary<string, Bitmap> _imageResDict = new();
    }
}