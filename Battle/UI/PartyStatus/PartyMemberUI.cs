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
    private InflictStatusContainerUI _inflictStatusUI;
    private List<ModifierStatsInflictStatusUI> _modifierStatsInflictsUI = new List<ModifierStatsInflictStatusUI>(); 

    public override void _Ready()
    {
        _inflictStatusUI = GetNode<InflictStatusContainerUI>("InflictStatusContainerUI");
        _memberIconUI = GetNode<TextureRect>("MemberIconUI");
        _memberHpUI = GetNode<Label>("MemberHpUI");
        _memberSpUI = GetNode<Label>("MemberSpUI");
    }

    public override void _Process(double delta)
    {
        if(!_isActive)
        {
            return;
        }

        _memberHpUI.Text = _partyMember.DataContainer.Hp.ToString() + "/" + _partyMember.DataContainer.HpMax.ToString();
        _memberSpUI.Text = _partyMember.DataContainer.Sp.ToString() + "/" + _partyMember.DataContainer.SpMax.ToString();
    }

    public void Setup(Character partyMember)
    {
        _partyMember = partyMember;
        // _memberIconUI
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
}