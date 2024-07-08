using Godot;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;

public partial class PlayerExplorationJumpState : PlayerExplorationBaseState
{
    public const float DEFAULT_JUMP_FORCE = 4f;
    private Vector3 _jumpDirection;
    private float _jumpForceAddition = 0;

    private float _inicialPosY;
    private float _finalPosY;

    public float JumpForceAddition {get { return _jumpForceAddition;} set { _jumpForceAddition = value;}}

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        //_jumpDirection = Context.Direction; 
        //float cancelCurrentSpeed = context.CurrentSpeed;
        //Context.FinalDirection = new Vector3(Context.FinalDirection.X - cancelCurrentSpeed * JUMP_MOVE_SPEED, Context.FinalDirection.Y, Context.FinalDirection.Z - cancelCurrentSpeed * JUMP_MOVE_SPEED);
        Context.CanMove = false;
        Context.JumpForce = DEFAULT_JUMP_FORCE; 
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

        if(Input.IsActionJustPressed("jump") )
        {
            if(Context.IsClimbingCheckerColliding())
            {
                Context.SwitchState(Context.climbingState);
            }
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

        if(!Context.IsFloorCheckerColliding())
        {
            Context.IsFreelooking = false;
        }

        // Fall State Checker
        if(Context.GetSlideCollisionCount() > 0 && Context.PreviousCollisionObject3D == Context.GetSlideCollision(0))
        {
            // We let the player move, in this case.
            Context.CanMove = true;
            GD.Print("Changing to fall state");
            Context.SwitchState(Context.fallState);
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
