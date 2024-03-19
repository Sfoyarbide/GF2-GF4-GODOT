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
    private bool _inCombat;
    private bool _inAction;

    private OneMoreManager _oneMoreManager;

    public static event EventHandler<OnBattleStartEventArgs> OnBattleStart;
    public static event EventHandler OnTurnEnd;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnCurrentCharacterChanged;
    public static event EventHandler<OnCurrentCharacterChangedEventArgs> OnOneMore;
    public event EventHandler<OnActionExecuteEventArgs> OnActionExecute;
    public event EventHandler<OnSelectionStartedEventArgs> OnSelectionStarted;
    public event EventHandler OnItemSelectionStarted;
    public event EventHandler<OnSkillSelectionStartedEventArgs> OnSkillSelectionStarted;
    public class OnBattleStartEventArgs : EventArgs
    {
        public List<Character> partyList;
        public List<Character> enemyList;
    }
    public class OnCurrentCharacterChangedEventArgs : EventArgs
    {
        public Character currentCharacter;
    }
    public class OnActionExecuteEventArgs : EventArgs
    {
        public BaseAction action;
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

    public override void _Ready()
    {
        BattleUI.OnActionSelectedChanged += BattleUI_OnActionSelectedChanged;
        KnockDown.CurrentCharacterKnockdown += KnockDown_CurrentCharacterKnockdown;

        // TEMP
        List<Character> newCharacterTurnList = new List<Character>();
        _characterTurnList = new List<Character>();
        _allyList = new List<Character>();
        _enemyList = new List<Character>();
        for(int x = 0; x < GetChild(0).GetChildCount(); x++)
        {
            newCharacterTurnList.Add(GetChild(0).GetChild(x) as Character);
        }

        _oneMoreManager = GetNode<OneMoreManager>("ManagerContainer/OneMoreManager");
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");

        SetupBattle(newCharacterTurnList);

        // Debug
        GD.Print("Character Turn List count: " + _characterTurnList.Count);
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
                case MeleeAction attackAction:
                    ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptor());
                    break;
                case SkillAction skillAction:
                    Skill skill = skillAction.CurrentSkill;
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
                case ItemAction itemAction:
                    if(!itemAction.CurrentItem.ForAllReceptors)
                    {    
                        ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptor());
                    }
                    else
                    {
                        ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptorList());
                    }
                    break;
                case IndividualPressionAction:
                    ExecuteAction(_battleDatabase.CharacterReceptorSelector.GetCharacterReceptor());
                    break;
            }
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

    public void ExecuteAction(Character characterReceptor)
    {
        if(_inAction)
        {
            return;
        }

        _inAction = true; 
        GetCurrentCharacter().DataContainer.SelectedAction.TakeAction(characterReceptor, CheckOneMore);
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
        GetCurrentCharacter().DataContainer.SelectedAction.TakeAction(characterReceptorList, CheckOneMore);
        _battleDatabase.CharacterReceptorSelector.CompleteSelection();
    
        OnActionExecute?.Invoke(this, new OnActionExecuteEventArgs{
            action = GetCurrentCharacter().DataContainer.SelectedAction
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

            OnOneMore?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
                currentCharacter = GetCurrentCharacter()
            });

            OnCurrentCharacterChanged?.Invoke(this, new OnCurrentCharacterChangedEventArgs{
                currentCharacter = GetCurrentCharacter()
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

        OnBattleStart?.Invoke(this, new OnBattleStartEventArgs{
            partyList = AllyList,
            enemyList = EnemyList
        });

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

    private void KnockDown_CurrentCharacterKnockdown(object sender, EventArgs e)
    {
        NextTurn();
    }
}