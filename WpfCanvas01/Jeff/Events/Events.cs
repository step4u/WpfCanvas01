using System.Windows.Input;

namespace Jeff.Events
{
    public delegate void MouseEnteredRotateRangeEventHandler(object source, MouseEventArgs e);
    public delegate void MouseLeftRotateRangeEventHandler(object source, MouseEventArgs e);

    public delegate void MouseEnteredResizeRangeEventHandler(object source, MouseEventArgs e);
    public delegate void MouseLeftResizeRangeEventHandler(object source, MouseEventArgs e);

    class CustomEvents
    {
    }
}
