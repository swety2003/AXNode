using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using XLib.Base;

namespace XLib.AvaloniaControl;

public static class Keyboard
{
    public static KeyModifiers Modifiers { get; private set; }
    
    public static void RegisterModifiers(KeyModifiers modifiers) => Modifiers = modifiers;
}
public static class Mouse
{
    public static PointerEventArgs? _pointerEventArgs { get; private set; }
    public static DragEventArgs? _dragEventArgs { get; private set; }
    public static void RegisterPointerEventArgs(PointerEventArgs pointerEventArgs) => _pointerEventArgs = pointerEventArgs;
    public static void RegisterDragEventArgs(DragEventArgs e) => _dragEventArgs = e;
    public static IPointer Pointer => _pointerEventArgs.Pointer;
    public static Point GetPosition(Control c,bool drag = false)
    {
        
        return drag? _dragEventArgs.GetPosition(c): _pointerEventArgs.GetPosition(c);
        // return Pointer.GetCurrentPoint(c).Position;
    }

    public static Control DirectlyOver {get; private set;}
    
    private static IDropable? _control;

    public static void InitDropable(IDropable? control)
    {
        _control = control;
    }
    
    public static void RegisterDirectlyOver(PointerEventArgs p)
    {
        if (_control is Control c)
        {
            var point = p.GetPosition(c);
            if (c.Bounds.Contains(point))
            {
                DirectlyOver = c;
            }
            else
            {
                DirectlyOver = null;
            }

        }
        
    }

    public static void ReleaseMouseCapture(this Control control)
    {
        Pointer.Capture(null);
    }

    public static void CaptureMouse(this Control control)
    {        
        Pointer.Capture(control);
    }
}