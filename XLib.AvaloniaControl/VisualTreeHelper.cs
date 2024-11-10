using Avalonia.Controls;

namespace XLib.AvaloniaControl;

public class VisualTreeHelper
{
    public static Control GetParent(Control element)
    {
        return element.Parent as Control;
    }
}