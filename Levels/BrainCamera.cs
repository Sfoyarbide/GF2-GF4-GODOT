using Godot;
using System;

public partial class BrainCamera : Node3D
{
    private Camera3D _camera3D;
    private Tween _tween;
    private bool _isTransitioning;

    public override void _Ready()
    {
        _camera3D = GetNode<Camera3D>("Camera3D");
        BattleCameraManager.CameraChanged += BattleCameraManager_CameraChanged;
    }

    public void SwitchCamera(Camera3D from, Camera3D to)
    {
        from.Current = false;
        to.Current = true;
    }

    public async void TransitionCamera(Camera3D from, Camera3D to, float duration)
    {
        if(_isTransitioning)
        {
            return; 
        }

        _camera3D.Fov = from.Fov;
        _camera3D.CullMask = from.CullMask;
        _camera3D.GlobalTransform = _camera3D.GlobalTransform;

        _camera3D.Current = true;

        _isTransitioning = true;

        _tween = CreateTween();
        _tween.SetParallel(true);
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Cubic);
        _tween.TweenProperty(_camera3D, "global_transform", to.GlobalTransform, duration).From(from.GlobalTransform);
        _tween.TweenProperty(_camera3D, "fov", to.Fov, duration).From(from.Fov);

        await ToSignal(_tween, Tween.SignalName.Finished);

        to.Current = true;
        _isTransitioning = false;
    }

    private void BattleCameraManager_CameraChanged(object sender, BattleCameraManager.CameraChangedEventArgs e)
    {
        TransitionCamera(e.from, e.to, e.transitionDuration);
    }
}