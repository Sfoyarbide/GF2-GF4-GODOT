using Godot;
using System;

public partial class PlayerExplorationJumpState : PlayerExplorationBaseState
{
    private Vector3 _jumpDirection;
    private float _jumpForceAddition = 0;

    private float _inicialPosY;
    private float _finalPosY;

    public float JumpForceAddition {get { return _jumpForceAddition;} set { _jumpForceAddition = value;}}

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        //_jumpDirection = Context.Direction;
        Context.CanMove = false;
        
        // Obtain inicial the y pos from where the jump will be made.
        _inicialPosY = Context.GlobalPosition.Y;

        // It will only be freelooking, if it is a floor jump and not a air jump. 
        if(context.IsFloorCheckerColliding())
        {
            Context.IsFreelooking = true;
        }

        context.Jump(context.JumpForce + _jumpForceAddition);
        _jumpForceAddition = 0;
    }

    public override void OnUpdate(float delta)
    {
        if(Context.Head.Position.Y < Context.yHeadStandPosition)
        {
            Context.LerpHead(Context.yHeadStandPosition, delta*2);
        }

        if(!Context.Camera3D.Rotation.Equals(Vector3.Zero))
        {
            Context.Camera3D.Rotation = Context.LerpVector(Context.Camera3D.Rotation, "z", Mathf.DegToRad(0), delta);
        }

        if(Input.IsActionJustPressed("jump"))
        {
            if(Context.IsWallCheckerColliding())
            {
                Context.SwitchState(Context.wallState);
            }
        }

        if(!Context.IsFloorCheckerColliding())
        {
            Context.IsFreelooking = false;
        }

        //Context.Direction = _jumpDirection;

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
        return "Jump";
    }
}
