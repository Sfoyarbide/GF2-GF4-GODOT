using Godot;
using System;

public partial class PlayerExplorationWallState : PlayerExplorationBaseState
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

        if(!Context.IsOnFloor() && !Context.IsFloorCheckerColliding() && !Context.IsWallCheckerColliding())
        {
            Context.SwitchState(Context.fallState);
            return;
        }

        Context.Direction = _jumpDirection;
    }

    public override string ToString()
    {
        return "Wall";
    }
}
