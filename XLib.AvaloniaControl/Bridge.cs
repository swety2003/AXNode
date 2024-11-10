using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace XLib.AvaloniaControl;

public static class Keyboard
{
    public static KeyModifiers Modifiers { get; private set; }
    
    public static void RegisterModifiers(KeyModifiers modifiers) => Modifiers = modifiers;
}
public static class Mouse
{
    public static PointerEventArgs? _pointerEventArgs { get; private set; }
    public static void RegisterPointerEventArgs(PointerEventArgs pointerEventArgs) => _pointerEventArgs = pointerEventArgs;
    public static IPointer Pointer => _pointerEventArgs.Pointer;
    public static Point GetPosition(Control c)
    {
        return _pointerEventArgs.GetPosition(c);
        // return Pointer.GetCurrentPoint(c).Position;
    }

    public static Control DirectlyOver => throw new NotImplementedException();

    public static void ReleaseMouseCapture(this Control control)
    {
        Pointer.Capture(null);
    }

    public static void CaptureMouse(this Control control)
    {        
        Pointer.Capture(control);
    }
}