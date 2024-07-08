using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node3D
{
    // Variables declatation.
    private BattleDatabase _battleDatabase;
    private List<Character> _characterTurnList;
    private List<Character> _allyList; 
    private List<Character> _enemyList;
    private int _indexTurn = 0;

    // Main Vars
    private bool _inCombat;
    private bool _inAction;

    private OneMoreManager _oneMoreManager;

    public static event EventHandler<OnBattleStartEventArgs> OnBattleStart;
    public static event EventHandler<OnBattleEndEventArgs> OnBattleEnd;
    public static event EventHandler OnTurnEnd;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnCurrentCharacterChanged;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnOneMore;
    public static event EventHandler<OnActionExecutedEventArgs> OnActionExecuted;
    public static event EventHandler<OnSelectionStartedEventArgs> OnSelectionStarted;
    public static event EventHandler OnItemSelectionStarted;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnCharacterTurnListChanged;
    public static event EventHandler<OnSkillSelectionStartedEventArgs> OnSkillSelectionStarted;
    public class OnBattleStartEventArgs : EventArgs
    {
        public List<Character> partyList;
        public List<Character> enemyList;
        public List<Character> characterTurnList;
    }
    public class OnBattleEndEventArgs : EventArgs
    {
        public bool win;
    }
    public class OnActionExecutedEventArgs : EventArgs
    {
        public Character character;
        public BaseAction selectedAction;
        public Action onActionCompleted;
        public List<Character> receptorList;
        public List<Character> allyList;
    }
    public class OnCurrentCharacterChangedEventArgs : EventArgs
    {
        public Character currentCharacter;
        public List<Character> partyList;
        public List<Character> enemyList;
        public bool inCombat;
    }
    public class OnSelectionStartedEventArgs : EventArgs
    {
        public BaseAction action;
        public bool allReceiveDamage;
    }
    public class OnSkillSelectionStartedEventArgs : EventArgs
    {
        public Character character;
    }

    public List<Character> CharacterTurnList
    {
        get { return _characterTurnList; }
    }

    public List<Character> AllyList 
    {
        get{ return _allyList; }
    }

    public List<Character> EnemyList 
    {
        get{ return _enemyList; }
    }

    public bool IsInCombat
    {
        get {return _inCombat;}
    }

    public override void _Ready()
    {
        // Finding necessary nodes. 
        _oneMoreManager = GetNode<OneMoreManager>("ManagerContainer/OneMoreManager");
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");

        // Subcribing to events.
        ActionButton.OnActionButtonDown += ActionButton_OnActionButtonDown;
        BattleStarter.OnBattleCharacterSetupFinished += BattleStarter_OnBattleCharacterSetupFinished;
        BaseAction.CannotTakeAction += BaseAction_CannotTakeAction;
        CharacterData.OnDie += CharacterData_OnDie;
        KnockDown.CurrentCharacterKnockdown += KnockDown_CurrentCharacterKnockdown;
        AIManager.OnAIReadyToDoAction += AIManager_OnAIReadyToDoAction;

        _inCombat = false;

        // TEMP
        /*
        List<Character> newCharacterTurnList = new List<Character>();
        _characterTurnList = new List<Character>();
        _allyList = new List<Character>();
        _enemyList = new List<Character>();
        for(int x = 0; x < GetChild(0).GetChildCount(); x++)
        {
            newCharacterTurnList.Add(GetChild(0).GetChild(x) as Character);
        }
        */
    }

    public override void _Process(double delta)
    {
        if(!_inCombat)
        {
            return;
        }

        if(_inAction)
        {
            return;
        }

        BaseAction selectedAction = GetCurrentCharacter().DataContainer.SelectedAction;

        if(Input.IsActionJustPressed("confirm"))
        {
            ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptorList());
            return;
        }

        if(Input.IsActionJustPressed("cancel"))
        {
            _battleDatabase.CharacterReceptorSelector.CancelSelection();
            return;
        }

        if(Input.IsActionJustPressed("select"))
        {
            switch(selectedAction)
            {
                case ItemAction:
                    OnItemSelectionStarted?.Invoke(this, EventArgs.Empty);
                    break;
                case SkillAction:
                    OnSkillSelectionStarted?.Invoke(this, new OnSkillSelectionStartedEventArgs
                    {
                        character = GetCurrentCharacter()
                    });
                    break;
                default:
                    OnSelectionStarted?.Invoke(this, new OnSelectionStartedEventArgs{
                        action = selectedAction,
                    });
                    break;
            }
            return;
        }
    }

    public void ExecuteAction(List<Character> characterReceptorList)
    {
        if(_inAction)
        {
            return;
        }

        _inAction = true;

        OnActionExecuted?.Invoke(this, new OnActionExecutedEventArgs{
            character = GetCurrentCharacter(),
            selectedAction = GetCurrentCharacter().DataContainer.SelectedAction,
            onActionCompleted = CheckOneMore,
            receptorList = characterReceptorList,
            allyList = _allyList
        });
    }

    public void CheckOneMore()
    {
        // One More System.
        if(_oneMoreManager.HaveOneMore())
        {
            UpdateAllyList();
            UpdateEnemyList();

            _inAction = false;

            if(!_inCombat)
            {
                return;
            }

            OnOneMore?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
                currentCharacter = GetCurrentCharacter()
            });

            OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
                currentCharacter = GetCurrentCharacter(),
                partyList = _allyList,
                enemyList = _enemyList,
                inCombat = _inCombat
            });
        }
        else
        {
            NextTurn();
        }
    }

    public void NextTurn()
    {
        UpdateAllyList();
        UpdateEnemyList();

        // CheckBattle();

        if(!_inCombat)
        {
            return;
        }

        DequeueCurrentCharacter();
        _inAction = false; 

        // If somebody is dead, it will skip his turn.
        if(GetCurrentCharacter().DataContainer.Hp <= 0)
        {
            NextTurn();
            return;
        }

        OnTurnEnd?.Invoke(this, EventArgs.Empty);

        OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
            currentCharacter = GetCurrentCharacter(),
            partyList = AllyList,
            enemyList = EnemyList,
            inCombat = _inCombat
        });
    }

    public void SetupBattle(List<Character> newCharacterTurnList)
    {
        _characterTurnList = newCharacterTurnList;
        _inCombat = true;

        OnBattleStart?.Invoke(this, new OnBattleStartEventArgs{
            partyList = _allyList,
            enemyList = _enemyList,
            characterTurnList = _characterTurnList
        });

        OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
            currentCharacter = GetCurrentCharacter(),
            partyList = _allyList,
            enemyList = _enemyList,
            inCombat = _inCombat
        });
    }

    public void AddQueueCharacter(Character newCharacter)
    {
        AddCharacterInTurns(newCharacter, _characterTurnList.Count);
    }

    public void DequeueCurrentCharacter()
    {
        Character character = _characterTurnList[0];
        RemoveCharacterInTurns(0);
        AddCharacterInTurns(character, _characterTurnList.Count);
    }

    public void AddCharacterInTurns(Character newCharacter, int index)
    {
        _characterTurnList.Insert(index, newCharacter);
    }

    public void RemoveCharacterInTurns(int index)
    {
        _characterTurnList.RemoveAt(index);
    }

    public Character GetCurrentCharacter()
    {
        if(!_inCombat)
        {
            return null;
        }
        return _characterTurnList[0];
    }

    public void UpdateEnemyList()
    {
        _enemyList = CombatCalculations.ObtainCharacterListByIsEnemy(_characterTurnList, true);
        if(_enemyList.Count == 0)
        {
            Win();
        }
    }

    private void Win()
    {
        OnBattleEnd?.Invoke(this, new OnBattleEndEventArgs{
            win = true
        });

        _inCombat = false;
        _inAction = false;
        _allyList.Clear();
        _enemyList.Clear();
        _characterTurnList.Clear();
    }

    public void UpdateAllyList()
    {
        _allyList = CombatCalculations.ObtainCharacterListByIsEnemy(_characterTurnList, false);
    }

    private void UpdateSelectedActionByAttack(Character character, Attack attack)
    {
        switch (attack)
        {
            case MeleeAttack:
                character.DataContainer.SelectedAction = character.DataContainer.ActionList[0];
                break;
            case Skill:
                character.DataContainer.SelectedAction = character.DataContainer.ActionList[1];
                SkillAction skillAction = (SkillAction)character.DataContainer.ActionList[1];
                skillAction.CurrentSkill = (Skill)attack;
                break;
        }
    }

    public void DebugTool_EndBattle()
    {
        int index = 0;
        foreach(Character character in _characterTurnList)
        {
            if(character.DataContainer.IsEnemy)
            {
                _characterTurnList.RemoveAt(index);
            }
            index++;
        }

        Win();
    }

    public void DebugTool_AddCharacter(Character character)
    {
        AddCharacterInTurns(character, _characterTurnList.Count-1);
        UpdateAllyList();
        UpdateEnemyList();

        OnCharacterTurnListChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
            currentCharacter = GetCurrentCharacter(),
            partyList = _allyList,
            enemyList = _enemyList
        });
    }

    public void DebugTool_RemoveCharacter(int index)
    {
        _characterTurnList[index].DataContainer.Hp = 0;
        //_characterTurnList.RemoveAt(_characterTurnList.FindIndex(c => c == character));
    }

    private void BaseAction_CannotTakeAction(object sender, EventArgs e)
    {
        _inAction = false;
    }

    private void ActionButton_OnActionButtonDown(object sender, ActionButton.OnActionButtonDownEventArgs e)
    {
        GetCurrentCharacter().DataContainer.SelectedAction = GetCurrentCharacter().DataContainer.ActionList[e.actionIndex];
    }

    private void KnockDown_CurrentCharacterKnockdown(object sender, EventArgs e)
    {
        NextTurn();
    }

    private void CharacterData_OnDie(object sender, CharacterData.CharacterDataEventArgs e)
    {
        if(e.character.DataContainer.IsEnemy)
        {
            RemoveCharacterInTurns(_characterTurnList.FindIndex(c => c.DataContainer.Character == e.character));
            e.character.DataContainer.SkillList.Clear();
        }
    }

    // The event that indicates the battle has begun.
    private void BattleStarter_OnBattleCharacterSetupFinished(object sender, BattleStarter.OnBattleCharacterSetupFinishedEventArgs e)
    {
        _allyList = e.partyMembers;
        _enemyList = e.enemyMembers;

        SetupBattle(e.characterTurnList);
    }

    // The event that sends the execution for the AI.
    private void AIManager_OnAIReadyToDoAction(object sender, AIManager.OnAIReadyToDoActionEventArgs e)
    {
        UpdateSelectedActionByAttack(GetCurrentCharacter(), e.attack);
        ExecuteAction(e.receptorList);
    }
}