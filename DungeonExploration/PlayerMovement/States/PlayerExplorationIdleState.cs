using Godot;
using System;

public partial class PlayerExplorationIdleState : PlayerExplorationBaseState
{
    private const float DEFAULT_MOVE_SPEED = 5f;

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.CanMove = true;
        Context.CurrentSpeed = DEFAULT_MOVE_SPEED;
        Context.StandCollisionShape.Disabled = false;
        Context.CrouchCollisionShape.Disabled = true;
    }

    public override void OnUpdate(float delta)
    {
        if(Context.Direction != Vector3.Zero)
        {
            Context.SwitchState(Context.walkingState);
        }

        if(Input.IsActionPressed("freelook"))
        {
            Context.IsFreelooking = true;
        }
        else
        {
            Context.IsFreelooking = false;
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

        if(Input.IsActionJustPressed("crouch") && Context.IsOnFloor())
        {
            Context.SwitchState(Context.crouchingState);            
            return;
        }

        if(Context.Head.Position.Y < Context.yHeadStandPosition)
        {
            Context.LerpHead(Context.yHeadStandPosition, delta);
        }
    }

    public override string ToString()
    {
        return "Idle";
    }
}