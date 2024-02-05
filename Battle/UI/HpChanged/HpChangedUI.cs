using Godot;
using System;

public partial class HpChangedUI : Node3D
{
    private Label3D _hpTextUI;
    private Node3D _pivot;
    private Sprite3D _damageUI;
    private Sprite3D _healUI;
    private bool _isActive;
    private Vector3 _dirToCamera;
    private Camera3D _camera;

    public override void _Ready()
    {
        _hpTextUI = GetNode<Label3D>("Pivot/HpTextUI");
        _damageUI = GetNode<Sprite3D>("Pivot/DamageUI");
        _healUI = GetNode<Sprite3D>("Pivot/HealUI");
        _pivot = GetNode<Node3D>("Pivot");
    }

    public override void _Process(double delta)
    {
        if(!_isActive)
        {
            return;
        }

        _dirToCamera = (_camera.GlobalPosition - GlobalPosition).Normalized();
        LookAt(GlobalPosition + _dirToCamera * -1);
    }

    public void Setup(int difference, Transform3D transform, bool isLessThanBefore)
    {
        Transform = transform;
        _camera = GetTree().Root.GetCamera3D();
        _dirToCamera = (_camera.GlobalPosition - GlobalPosition).Normalized();
        LookAt(GlobalPosition + _dirToCamera * -1);

        if(isLessThanBefore)
        {
            _hpTextUI.Text = difference.ToString();
            _damageUI.Show();
        }
        else
        {
            _hpTextUI.Text = (-difference).ToString();
            _healUI.Show();
        }
    }

    private void OnLifetimeTimeout()
    {
        QueueFree();
    }
}