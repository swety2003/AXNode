using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace AXNode.Interactivity;

public class TreeViewDragBehavior: Behavior<TreeView>
{
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<string> OnPointerEnterProperty =
        AvaloniaProperty.Register<TreeViewDragBehavior, string>(nameof(OnPointerEnter));

    public string OnPointerEnter
    {
        get => GetValue(OnPointerEnterProperty);
        set => SetValue(OnPointerEnterProperty, value);
    }
    
    protected override void OnAttached()
    {
        this.AssociatedObject.PointerPressed += async (s, e) =>
        {
            DataObject obj = new DataObject();
            obj.Set("object" , (s as Control).DataContext);
            await DragDrop.DoDragDrop(e, obj, DragDropEffects.Copy);
        };
        base.OnAttached();
    }
}