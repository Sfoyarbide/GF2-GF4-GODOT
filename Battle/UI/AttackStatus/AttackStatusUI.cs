using Godot;
using System;
using System.Diagnostics;

public partial class AttackStatusUI : Node3D
{
    private Label3D _damageTextUI;
    private Label3D _inflictTextUI;
    private Node3D _pivot;
    private Sprite3D _damageUI;
    private Sprite3D _healUI;
    private bool _isActive;
    private Vector3 _dirToCamera;
    private Camera3D _camera;

    public override void _Ready()
    {
        _damageTextUI = GetNode<Label3D>("Pivot/DamageTextUI");
        _inflictTextUI = GetNode<Label3D>("Pivot/InflictTextUI");
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

    public void Setup(BaseAction.AttackStateEventArgs e, Transform3D transform)
    {
        Transform = transform;
        _camera = GetTree().Root.GetCamera3D();
        _dirToCamera = (_camera.GlobalPosition - GlobalPosition).Normalized();
        LookAt(GlobalPosition + _dirToCamera * -1);

        if(e.isHit)
        {
            if(e.isPressionAttack)
            {
                _damageTextUI.Text = e.damage.ToString();
                _damageUI.Show();
                return;
            }

            switch(e.attack)
            {
                case ModifierSkill modifierSkill:
                    _inflictTextUI.Text = modifierSkill.ModifierStatsInflict.Name;
                    return;
                case InflictSkill inflictSkill:
                    _inflictTextUI.Text = inflictSkill.InflictState.Name;
                    return;
                case HealSkill healSkill:
                    _damageTextUI.Text = healSkill.Damage.ToString();
                    _healUI.Show();
                    return;
            }

            string elementStatus = "";
            if(e.receptor.DataContainer.AttackElementStatusDictionary.TryGetValue(e.attack.AttackType, out ElementStatus value))
            {
                switch(value)
                {
                    case ElementStatus.Weakness:
                        elementStatus = "Weak";
                        break;
                }
            }

            if(elementStatus != "")
            {
                _inflictTextUI.Text = elementStatus;
            }
            else if(e.inflictState != null)
            {
                _inflictTextUI.Text = e.inflictState.Name;
            }

            _damageTextUI.Text = e.damage.ToString();
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