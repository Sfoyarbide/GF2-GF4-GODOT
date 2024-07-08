using Godot;
using System;

public partial class PlayerExplorationWallRunningState : PlayerExplorationBaseState
{
    private Vector3 _wallCollisionNormal;
    private Timer _enterStateTimer;
    private Timer _cooldownTimer;
    private Timer _jumpGapTimer;
    private bool _hasEnterState;
    private bool _inCooldown;
    private float _gravityIncrease = 1.5f;
    private float _inicialWallCheckerTargetPositionX;
    
    public bool InCooldown { get { return _inCooldown;}}

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.PreviousCollisionObject3D = Context.GetSlideCollision(0);
        _enterStateTimer.Start();
        // All the condictions from the enter state to make this state works.
    }

    public override void OnUpdate(float delta)
    {
        if(_hasEnterState == false)
        {
            return;
        }

        // If the collision count is more than zero, means that there is a wall.
        if(Context.GetSlideCollisionCount() > 0 && Context.CanMakeWallRunning())
        {
            _wallCollisionNormal = Context.GetSlideCollision(0).GetNormal();
            Context.PreviousCollisionObject3D = Context.GetSlideCollision(0);
        }
        else
        {
            if(_jumpGapTimer.IsStopped())
            {
                _jumpGapTimer.Start();
            }
            return;
        }

        if(Input.IsActionJustPressed("jump"))
        {
            Context.jumpState.JumpForceAddition = 1f - (Context.Gravity/PlayerExplorationFallState.NORMAL_GRAVITY_VALUE);
            _hasEnterState = false;
            _inCooldown = true;
            _cooldownTimer.Start();
            Context.Gravity = PlayerExplorationFallState.NORMAL_GRAVITY_VALUE;
            Context.SwitchState(Context.jumpState);
            return;
        }

        if(Context.Gravity >= PlayerExplorationFallState.NORMAL_GRAVITY_VALUE)
        {
            Context.Gravity = PlayerExplorationFallState.NORMAL_GRAVITY_VALUE;
        }
        else
        {
            Context.Gravity += _gravityIncrease * delta;
        }

        Context.FinalDirection = -_wallCollisionNormal;
    }

    private void OnEnterStateTimerTimeout()
    {
        if(!Context.CanMakeWallRunning())
        {
            Context.SwitchState(Context.fallState);
            return;
        }

        Context.CanMove = true;
        Context.IsFreelooking = false;
        Context.FinalDirection = Vector3.Zero;
        //Context.Direction = new Vector3(0, Context.Direction.Y, 0);
        Context.Direction = Vector3.Zero;
        Context.Gravity = 0.5f;
        _hasEnterState = true;
    }

    private void OnCooldownTimerTimeout()
    {
        _inCooldown = false;
    }

    private void OnJumpGapTimerTimeout()
    {
        // Fall state checker.
        _hasEnterState = false;
        _inCooldown = true;
        _cooldownTimer.Start();
        Context.SwitchState(Context.fallState);
    }

    public override void _Ready()
    {
        _enterStateTimer = GetNode<Timer>("EnterStateTimer");
        _cooldownTimer = GetNode<Timer>("CooldownTimer");
        _jumpGapTimer = GetNode<Timer>("JumpGapTimer");
    }

    public override string ToString()
    {
        return "Wall Running";
    }

}