using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.Win32;
using XLib.Base;
using AXNode.SubSystem.OptionSystem;
using AXNode.SubSystem.WindowSystem;

namespace AXNode.SubSystem.ProjectSystem
{
    /// <summary>
    /// 文件工具
    /// </summary>
    public class FileTool
    {
        private FileTool()
        {
            _projectFilter.TypeList.Add(new TypeInfo("节点项目", "xnode"));
            _projectFilter.TypeList.Add(new TypeInfo("节点项目", "json"));
        }
        public static FileTool Instance { get; } = new FileTool();

        /// <summary>
        /// 打开读取项目对话框
        /// </summary>
        public string OpenReadProjectDialog()
        {
            var file = "";
            Task.Run(async () =>
            {

                var topLevel = TopLevel.GetTopLevel(WM.Main);
                var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
                {
                    Title = "Open XNode File",
                    AllowMultiple = false,
                    FileTypeFilter = new []{  new FilePickerFileType("XNode文件") { Patterns = new[] { "*.xnode" } }}

                });
                
                if (files.Count >= 1)
                {
                    file = files[0].TryGetLocalPath();
                }
            }).Wait();
            
            return file;
        }

        /// <summary>
        /// 打开保存项目对话框
        /// </summary>
        public string OpenSaveProjectDialog(string fileName)
        {            
            var file = "";

            Task.Run(async () =>
            {
                var topLevel = TopLevel.GetTopLevel(WM.Main);

                // 启动异步操作以打开对话框。
                var _file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save XNode File",
                    FileTypeChoices = new []{  new FilePickerFileType("XNode文件") { Patterns = new[] { "*.xnode" } }}
                    
                });

                if (_file is not null)
                {
                    file = _file.Path.AbsolutePath;
                }
            });
            return file;

        }

        private readonly FileFilter _projectFilter = new FileFilter();
    }
}