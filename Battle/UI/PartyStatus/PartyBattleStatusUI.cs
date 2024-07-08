using Godot;
using System;
using System.Collections.Generic;

public partial class PartyBattleStatusUI : Node
{
    [Export]
    private PackedScene _partyMemberUIScene;
    private HBoxContainer _partyMembersContainerUI;

    public override void _Ready()
    {
        _partyMembersContainerUI = GetNode<HBoxContainer>("Panel/PartyMembersContainerUI");
        if(_partyMemberUIScene == null)
        {
            GD.PrintErr("Party Member UI Scene has not been assigned. Plese add it in Party Battle Status UI.");
        }

        BattleStarter.OnFindingPartyMembersFinished += BattleStarter_OnFindingPartyMembersFinished;
        //BattleManager.OnBattleEnd += BattleManager_OnBattleEnd;   
    }

    public void Setup(List<Character> partyList)
    {
        foreach(Character partyMember in partyList)
        {
            PartyMemberUI partyMemberUI = (PartyMemberUI)_partyMemberUIScene.Instantiate();
            _partyMembersContainerUI.AddChild(partyMemberUI);
            partyMemberUI.Setup(partyMember);
        }
    }

    public void CleanPartyBattleStatusUI()
    {
        foreach(PartyMemberUI partyMember in _partyMembersContainerUI.GetChildren())
        {
            partyMember.QueueFree();
        }
    }

    private void BattleManager_OnBattleStart(object sender, BattleManager.OnBattleStartEventArgs e)
    {
        Setup(e.partyList);
    }

    private void BattleStarter_OnFindingPartyMembersFinished(object sender, BattleStarter.OnBattleCharacterSetupFinishedEventArgs e)
    {
        Setup(e.partyMembers);
    }
}