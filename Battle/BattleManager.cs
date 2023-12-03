using Godot;
using System;
using System.Collections.Generic;

public partial class BattleManager : Node3D
{
    // TEMP
    [Export]
    private SkillResource skillResource; 
    private bool temp;

    // Variables declatation.
    private BattleDatabase _battleDatabase;
    private List<Character> _characterTurnList;
    private List<Character> _allyList; 
    private List<Character> _enemyList;
    private int _indexTurn = 0;
    private bool _inCombat;
    private bool _inAction;

    public event EventHandler OnBattleStart;
    public event EventHandler OnTurnEnd;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnCurrentCharacterChanged;
    public event EventHandler<OnActionExecuteEventArgs> OnActionExecute;
    public class OnCurrentCharacterChangedEventArgs : EventArgs
    {
        public Character currentCharacter;
    }
    public class OnActionExecuteEventArgs : EventArgs
    {
        public BaseAction action;
    }

    public List<Character> AllyList 
    {
        get{ return _allyList; }
    }

    public List<Character> EnemyList 
    {
        get{ return _enemyList; }
    }

    public override void _Ready()
    {
        BattleUI.OnActionSelectedChanged += BattleUI_OnActionSelectedChanged;
        //OnBattleStart?.Invoke(this, EventArgs.Empty);

        // TEMP
        List<Character> newCharacterTurnList = new List<Character>();
        _characterTurnList = new List<Character>();
        _allyList = new List<Character>();
        _enemyList = new List<Character>();
        for(int x = 0; x < GetChild(0).GetChildCount(); x++)
        {
            newCharacterTurnList.Add(GetChild(0).GetChild(x) as Character);
        }

        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");

        SetupBattle(newCharacterTurnList);
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
            switch(selectedAction)
            {
                case AttackAction attackAction:
                    ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptor());
                    break;
                case SkillAction skillAction:
                    SkillResource skill = GetCurrentCharacter().DataContainer.SelectedSkill;
                    if(skill.IsAllReceiveDamage)
                    {
                        ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptorList());
                    }
                    else
                    {
                        ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptor());
                    }
                    break;
                case DefendAction defendAction:
                    ExecuteAction(GetCurrentCharacter());
                    break;
            }
            return;
        }

        /* TEST FOR ALL ENEMYS
        if(Input.IsKeyPressed(Key.X) && !temp)
        {
            _characterTurnList[0].DataContainer.SelectedAction = _characterTurnList[0].DataContainer.ActionList[2];
            _characterTurnList[0].DataContainer.SelectedSkill = skillResource;
            List<Character> characterReceptors = new List<Character>();
            characterReceptors.Add(_characterTurnList[1]); 
            characterReceptors.Add(_characterTurnList[2]); 
            ExecuteAction(characterReceptors);
            temp = true;
        }
        */

        if(Input.IsActionJustPressed("cancel"))
        {
            _battleDatabase.CharacterReceptorSelector.CancelSelection();
            return;
        }

        if(Input.IsActionJustPressed("select"))
        {
            switch(selectedAction)
            {
                case AttackAction:
                    _battleDatabase.CharacterReceptorSelector.SetupSelection(false, false);
                    break;
                case SkillAction:
                    _battleDatabase.CharacterReceptorSelector.SetupSelection(skillResource.IsAllReceiveDamage, false);
                    break;
                case DefendAction:
                    _battleDatabase.CharacterReceptorSelector.SetupSelection(false, false, true);
                    break;
            }
            return;
        }
    }

    public void ExecuteAction(Character characterReceptor)
    {
        if(_inAction)
        {
            return;
        }

        _inAction = true; 
        GetCurrentCharacter().DataContainer.SelectedAction.TakeAction(characterReceptor, NextTurn);
        _battleDatabase.CharacterReceptorSelector.CompleteSelection();
        //EmitSignal(SignalName.ActionExecuted);
    }

    public void ExecuteAction(List<Character> characterReceptorList)
    {
        if(_inAction)
        {
            return;
        }

        _inAction = true;
        GetCurrentCharacter().DataContainer.SelectedAction.TakeAction(characterReceptorList, NextTurn);
        _battleDatabase.CharacterReceptorSelector.CompleteSelection();
    
        OnActionExecute?.Invoke(this, new OnActionExecuteEventArgs{
            action = GetCurrentCharacter().DataContainer.SelectedAction
        });

    }

    public void NextTurn()
    {
        UpdateAllyList();
        UpdateEnemyList();

        // CheckBattle();

        DequeueCurrentCharacter();
        _inAction = false; 

        OnTurnEnd?.Invoke(this, EventArgs.Empty);

        OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
            currentCharacter = GetCurrentCharacter()
        });
    }

    public void SetupBattle(List<Character> newCharacterTurnList)
    {
        _characterTurnList = newCharacterTurnList;
        UpdateAllyList();
        UpdateEnemyList();
        _inCombat = true;

        OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
            currentCharacter = GetCurrentCharacter()
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
        return _characterTurnList[0];
    }

    public void UpdateEnemyList()
    {
        _enemyList = CombatCalculations.ObtainCharacterListByIsEnemy(_characterTurnList, true);
    }

    public void UpdateAllyList()
    {
        _allyList = CombatCalculations.ObtainCharacterListByIsEnemy(_characterTurnList, false);
    }

    private void BattleUI_OnActionSelectedChanged(object sender, BattleUI.OnActionSelectedChangedEventArgs e)
    {
        GetCurrentCharacter().DataContainer.SelectedAction = GetCurrentCharacter().DataContainer.ActionList[e.actionIndex];
    }
}