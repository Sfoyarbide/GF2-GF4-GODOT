using Godot;
using System;
using System.Collections.Generic;

public partial class PartyMemberUI : Node
{
    private bool _isActive;
    private Character _partyMember;
    private TextureRect _memberIconUI;
    private Label _memberHpUI;
    private Label _memberSpUI;
    private Label _deadTextUI;
    private InflictStatusContainerUI _inflictStatusUI;
    private HSlider _memberPressionLevelUI;
    private List<ModifierStatsInflictStatusUI> _modifierStatsInflictsUI = new List<ModifierStatsInflictStatusUI>();

    public override void _Ready()
    {
        // Finding nodes.
        _inflictStatusUI = GetNode<InflictStatusContainerUI>("InflictStatusContainerUI");
        _memberIconUI = GetNode<TextureRect>("MemberIconUI");
        _memberHpUI = GetNode<Label>("MemberHpUI");
        _memberSpUI = GetNode<Label>("MemberSpUI");
        _deadTextUI = GetNode<Label>("DeadTextUI");
        _memberPressionLevelUI = GetNode<HSlider>("MemberPressionLevelUI");
    
        // Subcribing to events.
        CharacterData.OnHpChanged += CharacterData_OnHpChanged;
        BaseAction.AttackState += BaseAction_AttackState;
    }

    public override void _Process(double delta)
    {
        
    }

    public void Setup(Character partyMember)
    {
        _partyMember = partyMember;
        _memberPressionLevelUI.Value = partyMember.DataContainer.PressionLevel;
        _memberHpUI.Text = partyMember.DataContainer.Hp.ToString() + "/" + partyMember.DataContainer.HpMax.ToString();
        _memberSpUI.Text = partyMember.DataContainer.Sp.ToString() + "/" + partyMember.DataContainer.SpMax.ToString();
       
        Node modifierStatsInflictStatusContainers = GetNode<Node>("ModifierStatsInflictStatusContainers");
        for(int x = 0; x < modifierStatsInflictStatusContainers.GetChildCount(); x++)
        {
            ModifierStatsInflictStatusUI modifierStatsInflictStatusUI = (ModifierStatsInflictStatusUI)modifierStatsInflictStatusContainers.GetChild(x);
            modifierStatsInflictStatusUI.Setup(_partyMember);
        }

        _inflictStatusUI.Setup(partyMember);
       
        _isActive = true;
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        if(e.receptor != _partyMember && e.current != _partyMember)
        {
            return;
        }

        if(!_isActive)
        {
            return;
        }

        if(_partyMember.DataContainer.Hp <= 0)
        {
            _deadTextUI.Show();
        }
        else
        {
            _deadTextUI.Hide();
        }

        _memberHpUI.Text = _partyMember.DataContainer.Hp.ToString() + "/" + _partyMember.DataContainer.HpMax.ToString();
        _memberSpUI.Text = _partyMember.DataContainer.Sp.ToString() + "/" + _partyMember.DataContainer.SpMax.ToString();
        _memberPressionLevelUI.Value = _partyMember.DataContainer.PressionLevel;
    }

    private void CharacterData_OnHpChanged(object sender, CharacterData.OnHpChangedEventArgs e)
    {
        if(e.character != _partyMember)
        {
            return;
        }

        if(!_isActive)
        {
            return;
        }

        if(_partyMember.DataContainer.Hp <= 0)
        {
            _deadTextUI.Show();
        }
        else
        {
            _deadTextUI.Hide();
        }

        _memberHpUI.Text = _partyMember.DataContainer.Hp.ToString() + "/" + _partyMember.DataContainer.HpMax.ToString();
        _memberSpUI.Text = _partyMember.DataContainer.Sp.ToString() + "/" + _partyMember.DataContainer.SpMax.ToString();
        _memberPressionLevelUI.Value = _partyMember.DataContainer.PressionLevel;
    }

}