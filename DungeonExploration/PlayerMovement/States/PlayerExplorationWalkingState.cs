using Godot;
using System;

public partial class PlayerExplorationWalkingState : PlayerExplorationBaseState
{
    private float walkingSpeed = 5f;
    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.CurrentSpeed = walkingSpeed;
    }

    public override void OnUpdate(float delta)
    {
        if(Context.Direction == Vector3.Zero)
        {
            Context.SwitchState(Context.idleState);
        }

        if(Input.IsActionJustPressed("sprint") || Input.IsActionPressed("sprint"))
        {
            Context.SwitchState(Context.sprintingState);
        }

        if(Input.IsActionJustPressed("jump"))
        {
            if(Context.IsClimbingCheckerColliding())
            {
                Context.SwitchState(Context.climbingState);
            }
            else
            {
                Context.SwitchState(Context.jumpState);
            }
            return;
        }

        // Wall Running State Checker.
        if(Input.IsActionPressed("jump") && Context.CanMakeWallRunning())
        {
            if(Context.GetSlideCollisionCount() > 0)
            { 
                Context.SwitchState(Context.wallRunningState);
            }
            return;
        }

        if(!Context.IsOnFloor() && !Context.IsFloorCheckerColliding())
        {
            Context.SwitchState(Context.fallState);
            return;
        }

        if(Input.IsActionJustPressed("crouch"))
        {
            if(Context.IsOnFloor())
            {
                Context.SwitchState(Context.crouchingState);
            }
        }

        if(Context.Head.Position.Y < Context.yHeadStandPosition)
        {
            Context.LerpHead(Context.yHeadStandPosition, delta);
        }
    }

    public override string ToString()
    {
        return "Walking";
    }
}
