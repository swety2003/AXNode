using Avalonia.Controls;
using Microsoft.Win32;
using XLib.Base;
using AXNode.SubSystem.OptionSystem;

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
            // OpenFileDialog dialog = new OpenFileDialog
            // {
            //     InitialDirectory = OptionManager.Instance.ProjectPath,
            //     Filter = _projectFilter.ToString(),
            // };
            // return dialog.ShowDialog() == true ? dialog.FileName : "";
            return @"C:\Users\swety\Desktop\新建节点项目_01.xnode";
        }

        /// <summary>
        /// 打开保存项目对话框
        /// </summary>
        public string OpenSaveProjectDialog(string fileName)
        {
            // SaveFileDialog dialog = new SaveFileDialog
            // {
            //     InitialDirectory = OptionManager.Instance.ProjectPath,
            //     FileName = $"{fileName}.xnode",
            //     Filter = _projectFilter.ToString(),
            // };
            // return dialog.ShowDialog() == true ? dialog.FileName : "";
            return "";

        }

        private readonly FileFilter _projectFilter = new FileFilter();
    }
}