using Godot;
using System;
using System.Diagnostics;

public partial class AttackStatusUI : Node3D
{
    private Label3D _damageTextUI;
    private Node3D _pivot;
    private Sprite3D _damageUI;
    private Sprite3D _healUI;
    private bool _isActive;
    private Vector3 _dirToCamera;
    private Camera3D _camera;

    public override void _Ready()
    {
        _damageTextUI = GetNode<Label3D>("Pivot/DamageTextUI");
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

    public void Setup(int damage, bool isHit, Attack attack, Transform3D transform)
    {
        Transform = transform;
        _camera = GetTree().Root.GetCamera3D();
        _dirToCamera = (_camera.GlobalPosition - GlobalPosition).Normalized();
        LookAt(GlobalPosition + _dirToCamera * -1);
        
        switch(attack)
        {
            case ModifierSkill:
                return;
            case InflictSkill:
                return;
            case HealSkill healSkill:
                _damageTextUI.Text = healSkill.Damage.ToString();
                _healUI.Show();
                return;
        }

        if(isHit)
        {
            _damageTextUI.Text = damage.ToString();
            _damageUI.Show();
        }
        else
        {
            _damageTextUI.Text = "Miss";
            _damageUI.Show();
        }
    }

    private void OnLifetimeTimeout()
    {
        QueueFree();
    }
}