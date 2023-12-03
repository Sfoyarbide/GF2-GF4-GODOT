using Godot;
using System;
using System.Threading;

public partial class ActionButton : Button
{
    private int _actionIndex;
    private Label _label;
    public static event EventHandler<OnActionButtonDownEventArgs> OnActionButtonDown;
    public class OnActionButtonDownEventArgs : EventArgs
    {
        public int selectedActionIndex;
    }

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
    }

    public void SetupActionButton(int actionIndex, string actionName)
    {
        _actionIndex = actionIndex;
        _label.Text = actionName;
    }

    private void OnButtonDown()
    {
        OnActionButtonDown?.Invoke(this, new OnActionButtonDownEventArgs{
            selectedActionIndex = _actionIndex
        });
    }
}
