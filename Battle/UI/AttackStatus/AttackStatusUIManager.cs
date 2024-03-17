using Godot;
using System;

public partial class AttackStatusUIManager : Node
{
    [Export]
    private PackedScene _attackStatusUIScene;
    private Node3D _battleNode;

    public override void _Ready()
    {
        BaseAction.AttackState += BaseAction_AttackState;
        _battleNode = (Node3D)GetParent().GetParent();
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        ShowAttackStatusUI(e);
    }

    public void ShowAttackStatusUI(BaseAction.AttackStateEventArgs e)
    {
        AttackStatusUI attackStatusUI = (AttackStatusUI)_attackStatusUIScene.Instantiate();
        _battleNode.AddChild(attackStatusUI);
    
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

        attackStatusUI.Setup(e.damage, e.isHit, e.attack, e.inflictState, elementStatus, e.receptor.GetMarkerChildTransform(3));
    }

}