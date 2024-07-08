using Godot;
using System;
using System.Collections.Generic;


public partial class BattleStarter : Node
{
    // Vars.
    private BattleDatabase _battleDatabase;

    // Party members in exploration.
    private List<Character> _partyMembers = new List<Character>();
    
    // All enemies in the level. 
    private Node _enemiesInLevelParent;
    private List<Character> _enemiesInLevelList = new List<Character>();

    // TEMP
    private EnemyGroupInLevel _enemyGroupInLevel;

    public EnemyGroupInLevel EnemyGroupInLevel 
    {
        get{ return _enemyGroupInLevel; }
    }

    public static event EventHandler<OnBattleCharacterSetupFinishedEventArgs> OnFindingPartyMembersFinished;
    public static event EventHandler<OnBattleCharacterSetupFinishedEventArgs> OnBattleCharacterSetupFinished;
    public class OnFindingPartyMembersFinishedEventArgs : EventArgs
    {
        public List<Character> partyMembers;
    }
    public class OnBattleCharacterSetupFinishedEventArgs : EventArgs
    {
        public List<Character> characterTurnList;
        public List<Character> partyMembers;
        public List<Character> enemyMembers;
        public bool isAdvantage;
    }

    public override void _Ready()
    {
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");

        _enemiesInLevelParent = GetNode<Node>("EnemiesInLevel");
        for(int x = 0; x < _enemiesInLevelParent.GetChildCount(); x++)
        {
            Character enemy = (Character)_enemiesInLevelParent.GetChild(x);
            _enemiesInLevelList.Add(enemy);
        }

        //TEMP
        Node partyMembers = GetNode<Node>("PartyMembers");
        for(int x = 0; x < partyMembers.GetChildCount(); x++)
        {
            Character party = (Character)partyMembers.GetChild(x);
            _partyMembers.Add(party);
        }

        _enemyGroupInLevel = GetNode<EnemyGroupInLevel>("EnemyGroupInLevel");

        OnFindingPartyMembersFinished?.Invoke(this, new OnBattleCharacterSetupFinishedEventArgs{
            partyMembers = _partyMembers
        });
    }

    public override void _Process(double delta)
    {
        // TEMP
        if(Input.IsActionJustPressed("test") && !_battleDatabase.BattleManager.IsInCombat)
        {
            BattleCharacterSetup(true);
        }
    }

    private void ResetCharacterCombatData(Character character)
    {
        character.DataContainer.Hp = character.DataContainer.HpMax;
        character.DataContainer.Sp = character.DataContainer.SpMax;
    }

    private List<Character> GetEnemiesList(int groupIndex)
    {
        List<Character> enemiesList = new List<Character>();
        foreach(string enemyName in _enemyGroupInLevel.EnemyGroupInLevelList[groupIndex])
        {
            foreach(Character enemy in _enemiesInLevelList)
            {
                if(enemy.DataContainer.CharacterName == enemyName && !enemy.InCombat)
                {
                    ResetCharacterCombatData(enemy);
                    enemy.Show();
                    enemiesList.Add(enemy);
                    enemy.InCombat = true;
                    break;
                } 
            }
        }
        return enemiesList;
    }

    public void BattleCharacterSetup(bool isAdvantage, int randomGroup = -1)
    {
        if(randomGroup == -1)
        {
            randomGroup = GD.RandRange(0, _enemyGroupInLevel.EnemyGroupInLevelList.Count-1);
        }

        List<Character> characterTurnList = new List<Character>();
        List<Character> enemiesList = GetEnemiesList(randomGroup);

        if(isAdvantage)
        {
            characterTurnList.AddRange(_partyMembers);
            characterTurnList.AddRange(enemiesList);
        }
        else
        {
            characterTurnList.AddRange(enemiesList);
            characterTurnList.AddRange(_partyMembers);
        }

        GD.Print("New character turn list count: " + characterTurnList.Count);

        OnBattleCharacterSetupFinished?.Invoke(this, new OnBattleCharacterSetupFinishedEventArgs{
            characterTurnList = characterTurnList,
            partyMembers = _partyMembers,
            enemyMembers = enemiesList,
            isAdvantage = isAdvantage,
        });
    }

    public void DebugTool_SetupBattle(bool isAdvantage, int randomGroup)
    {
        BattleCharacterSetup(isAdvantage, randomGroup);
    }
}