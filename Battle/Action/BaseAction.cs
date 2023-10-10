using Godot;
using System.Collections.Generic;
using System;

public abstract partial class BaseAction : Node3D
{
    private Character _character;
    private Action _onActionComplete;
    public abstract void TakeAction(Character characterReceptor, Action onActionComplete);
    public virtual void TakeAction(List<Character> characterReceptorList, Action onActionComplete){}

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

    public abstract string GetActionName();
}