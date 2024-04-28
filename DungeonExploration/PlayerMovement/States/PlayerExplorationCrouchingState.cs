using Godot;
using System;

public partial class PlayerExplorationCrouchingState : PlayerExplorationBaseState
{
    private float crouchSpeed = 3.8f;
    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        context.CurrentSpeed = crouchSpeed;
        Context.StandCollisionShape.Disabled = true;
        Context.CrouchCollisionShape.Disabled = false;
    }

    public override void OnUpdate(float delta)
    {
        if(!Input.IsActionPressed("crouch"))
        {
            if(!Context.CrouchChecker.IsColliding())
            {
                Context.SwitchState(Context.idleState);   
            }
        }

        if(!Context.IsOnFloor() && !Context.IsFloorCheckerColliding())
        {
            Context.SwitchState(Context.fallState);
            return;
        }

        if(!Context.Camera3D.Rotation.Z.Equals(0))
        {
            Context.Camera3D.Rotation = Context.LerpVector(Context.Camera3D.Rotation, "z", Mathf.DegToRad(0), delta);
        }

        if(Context.Head.Position.Y > Context.yPositionCrouchHead)
        {
            Context.LerpHead(Context.yPositionCrouchHead, delta);
        }
    }

    public override string ToString()
    {
        return "Crouching";
    }
}
