using Godot;
using System;

public partial class PlayerExplorationFallState : PlayerExplorationBaseState
{
    private const float FALLING_MOVE_SPEED = 4;
    public const float NORMAL_GRAVITY_VALUE = 9.8f;
    private float _inicialPosY;
    private float _finalPosY;

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        //Context.CurrentSpeed = FALLING_MOVE_SPEED;
        _inicialPosY = Context.GlobalPosition.Y;
        Context.Gravity = NORMAL_GRAVITY_VALUE;
    }

    public override void OnUpdate(float delta)
    {
        // Wall Running State Checker.
        if(Input.IsActionPressed("jump") && Context.CanMakeWallRunning())
        {
            if(Context.GetSlideCollisionCount() > 0)
            { 
                Context.SwitchState(Context.wallRunningState);
            }
            return;
        }

        // Climbing State Checker.
        if(Input.IsActionJustPressed("jump") && Context.IsFloorCheckerColliding())
        {
            if(Context.IsClimbingCheckerColliding())
            {
                Context.SwitchState(Context.climbingState);
            }
            return;
        }

        if(Context.IsOnFloor())
        {   
            _finalPosY = Context.GlobalPosition.Y;

            if(_inicialPosY - _finalPosY >= 3f)
            {
                Context.SwitchState(Context.rollState);
                return;
            }

            Context.SwitchState(Context.idleState);
        }
    }

    public override string ToString()
    {
        return "Fall";
    }
}
