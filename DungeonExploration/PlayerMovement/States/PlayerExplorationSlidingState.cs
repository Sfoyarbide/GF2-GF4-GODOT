using Godot;
using System;

public partial class PlayerExplorationSlidingState : PlayerExplorationBaseState
{
    private Vector3 _slideDirection;
    private Timer _sliderTimer;
    private float _slideSpeed = 10f;
    private const float SLIDE_JUMP_ADDITION = 2.5f; 
    private Vector3 _cameraSlidingRotation;

    public override void _Ready()
    {
        _sliderTimer = GetNode<Timer>("SliderTimer");
    }

    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.StandCollisionShape.Disabled = true;
        Context.CrouchCollisionShape.Disabled = false;
        Context.CurrentSpeed = (float)_sliderTimer.TimeLeft;
        Context.IsFreelooking = true;
        _cameraSlidingRotation = new Vector3(Context.Camera3D.Rotation.X, Context.Camera3D.Rotation.Y, Mathf.DegToRad(-7));
        _slideDirection = Context.Direction;
        _sliderTimer.Start();
    }

    public override void OnUpdate(float delta)
    {
        if(!Context.Camera3D.Rotation.Equals(_cameraSlidingRotation))
        {
            Context.Camera3D.Rotation = Context.LerpVector(Context.Camera3D.Rotation, "z", _cameraSlidingRotation.Z, delta);
        }

        Context.CurrentSpeed = (float)_sliderTimer.TimeLeft * _slideSpeed;
        Context.Direction = _slideDirection;

        if(Input.IsActionJustPressed("jump"))
        {
            Context.jumpState.JumpForceAddition = SLIDE_JUMP_ADDITION - (float)_sliderTimer.TimeLeft;
            Context.SwitchState(Context.jumpState);
            _sliderTimer.Stop();
            return;
        }

        if(Context.Head.Position.Y > Context.yPositionCrouchHead)
        {
            Context.LerpHead(Context.yPositionCrouchHead, delta);
        }
    }

    private void OnSliderTimerTimeout()
    {
        Context.IsFreelooking = false;
        Context.Camera3D.Rotation = new Vector3(Context.Camera3D.Rotation.X, Context.Camera3D.Rotation.Y, Mathf.DegToRad(0));
        
        if(!Context.IsOnFloor() && !Context.IsFloorCheckerColliding())
        {
            Context.SwitchState(Context.fallState);
        }
        else
        {
            Context.SwitchState(Context.crouchingState);
        }
    }

    public override string ToString()
    {
        return "Sliding";
    }
}
