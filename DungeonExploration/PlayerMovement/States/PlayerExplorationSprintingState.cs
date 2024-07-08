using Godot;
using System;

public partial class PlayerExplorationSprintingState : PlayerExplorationBaseState
{
    private float _sprintingSpeed = 8.5f;
    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.CurrentSpeed = _sprintingSpeed;
    }

    public override void OnUpdate(float delta)
    {
        if(!Input.IsActionPressed("sprint"))
        {
            Context.SwitchState(Context.walkingState);
            return;
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
        }

        if(Input.IsActionJustPressed("crouch"))
        {
            // It will only slide if is the floor
            if(Context.IsOnFloor())
            {
                Context.SwitchState(Context.slidingState);
            }
            /* I am not so sure about making the player crouch when is in the air.
            else
            {
                Context.SwitchState(Context.crouchingState);
            }
            */
            return;
        }

        if(Input.IsActionPressed("freelook"))
        {
            Context.IsFreelooking = true;
        }
        else
        {
            Context.IsFreelooking = false;
        }

        if(Context.Direction.Equals(Vector3.Zero))
        {
            Context.SwitchState(Context.idleState);
            return;
        }
    }
    public override string ToString()
    {
        return "Sprinting";
    }
}