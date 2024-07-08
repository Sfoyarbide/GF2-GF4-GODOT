using Godot;
using System;
using System.Collections.Generic;

public partial class DebugTools_Battle : Node
{

	private BattleDatabase _battleDatabase;
	private BattleStarter _battleStarter;
	private MenuButton _startBattle;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
		BattleStarter.OnFindingPartyMembersFinished += BattleStarter_OnFindingPartyMembersFinished;
		_startBattle = GetNode<MenuButton>("VBoxContainer/StartBattle");
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.



    public override void _Process(double delta)
	{
	}

	public void DebugTool_SetupBattleUI(BattleStarter battleStarter)
	{
		int index = 0;
		foreach(List<string> groups in battleStarter.EnemyGroupInLevel.EnemyGroupInLevelList)
		{
			string membersName = "";
			foreach(string members in groups)
			{
				membersName += ";" + members;
			}
			_startBattle.GetPopup().AddItem(membersName, index);
			index++;
		}
	}
	
    private void StartBattle_Popup_IndexPressed(long index)
    {
        _battleStarter.BattleCharacterSetup(true, (int)index);
    }

	private void BattleStarter_OnFindingPartyMembersFinished(object sender, BattleStarter.OnBattleCharacterSetupFinishedEventArgs e)
    {
		_battleStarter = (BattleStarter)sender;
		_startBattle.GetPopup().IndexPressed += StartBattle_Popup_IndexPressed;
		DebugTool_SetupBattleUI(_battleStarter);
	}
}
