using Godot;
using System;

public partial class ActionButton : Button
{
    // For knowing what to put in the action index, check the action container in the Character scene. 
    [Export]
    protected int _actionIndex;
    protected BattleUI _battleUI;
    public static event EventHandler<OnActionButtonDownEventArgs> OnActionButtonDown;
    public class OnActionButtonDownEventArgs : EventArgs
    {
        public int actionIndex;
    }

    public override void _Ready()
    {
        Pressed += () => {
            if(_battleUI.CanSelect)
            {
                OnActionButtonDown?.Invoke(this, new OnActionButtonDownEventArgs{
                    actionIndex = _actionIndex
                });
            }
        };
    }

    public void Setup(BattleUI battleUI)
    {
        _battleUI = battleUI;
    }
}
