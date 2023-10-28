using Godot;
using System.Collections.Generic;
using System;

public abstract partial class BaseAction : Node3D
{
    private Character _character;
    private Action _onActionComplete;
    private bool _inAction;
    private Timer _timer;
    public abstract void TakeAction(Character characterReceptor, Action onActionComplete);
    public virtual void TakeAction(List<Character> characterReceptorList, Action onActionComplete){}
    public abstract void EndingAction();

    public override void _Ready()
    {
        _timer = GetNode<Timer>("Timer");
    }

    public Character Character
    {
        get { return _character; }
        set { _character = value; }
    }

    public Action OnActionComplete
    {
        get { return _onActionComplete; }
        set { _onActionComplete = value; }
    }

    public bool InAction
    {
        get { return _inAction; }
        set { _inAction = value; }
    }

    public abstract string GetActionName();

    public Timer Timer
    {
        get { return _timer; }
        set { _timer = value; }
    }
}