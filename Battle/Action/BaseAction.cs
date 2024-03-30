using Godot;
using System.Collections.Generic;
using System;

public abstract partial class BaseAction : Node
{
    private Character _character;
    private Action _onActionComplete;
    private bool _inAction;
    private Timer _timer;
    public static event EventHandler<GenericBaseActionEventArgs> ActionTaken;
    public static event EventHandler<GenericBaseActionEventArgs> CannotTakeAction;
    public static event EventHandler<AttackStateEventArgs> AttackState;
    public class GenericBaseActionEventArgs : EventArgs
    {
        public BaseAction baseAction;
    }
    public class AttackStateEventArgs : EventArgs
    {
        public Character current;
        public Character receptor;
        public bool isHit;
        public int damage;
        public Attack attack;
        public bool isPressionAttack;
        public InflictState inflictState;
        public BaseAction baseAction;
    }

    public abstract void TakeAction(List<Character> characterReceptorList, Action onActionComplete);

    public abstract void EndingAction();
    public void OnAttackState(AttackStateEventArgs attackStateEventArgs)
    {
        AttackState?.Invoke(this, attackStateEventArgs);
    }

    public override void _Ready()
    {
        BattleManager.OnActionExecuted += BattleManager_OnActionExecuted;
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

    public Timer Timer
    {
        get { return _timer; }
        set { _timer = value; }
    }

    protected void OnActionTaken()
    {
        ActionTaken?.Invoke(this, new GenericBaseActionEventArgs{
            baseAction = this
        });
    }

    protected void OnCannotTakeAction()
    {
        CannotTakeAction?.Invoke(this, new GenericBaseActionEventArgs{
            baseAction = this
        });
    }

    public abstract string GetActionName();

    protected virtual void BattleManager_OnActionExecuted(object sender, BattleManager.OnActionExecutedEventArgs e)
    { 
        if(e.character == Character && e.selectedAction == this)
        {
            TakeAction(e.receptorList, e.onActionCompleted);
        }
    }
}