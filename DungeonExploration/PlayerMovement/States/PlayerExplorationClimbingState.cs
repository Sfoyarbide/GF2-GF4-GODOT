using Godot;
using System;

public partial class PlayerExplorationClimbingState : PlayerExplorationBaseState
{
    private Vector3 _jumpDirection;

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        _jumpDirection = Context.Direction;
        context.Jump(7f);
    }

    public override void OnUpdate(float delta)
    {
        if(Context.IsOnFloor())
        {   
            Context.SwitchState(Context.idleState);
        }

        if(Context.Head.Position.Y < Context.yHeadStandPosition)
        {
            Context.LerpHead(Context.yHeadStandPosition, delta*2);
        }

        if(!Context.Camera3D.Rotation.Equals(Vector3.Zero))
        {
            Context.Camera3D.Rotation = Context.LerpVector(Context.Camera3D.Rotation, "z", Mathf.DegToRad(0), delta);
        }

        if(!Context.IsOnFloor() && !Context.IsFloorCheckerColliding() && !Context.IsClimbingCheckerColliding())
        {
            Context.SwitchState(Context.fallState);
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

        Context.Direction = _jumpDirection;
    }

    public override string ToString()
    {
        return "Climbing";
    }
}
