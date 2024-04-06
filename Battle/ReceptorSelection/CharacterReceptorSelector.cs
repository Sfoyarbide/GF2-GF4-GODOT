using Godot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

public enum ReceptorCriteria
{
    Enemy,
    Ally,
    Self,
    IsBelowMaxHpAndAlive,
    WithoutSameModifierInflict,
    WithoutSameInflict, 
    Alive,
    Dead
}

public partial class CharacterReceptorSelector : Node3D
{
    private BattleDatabase _battleDatabase;
    private Character _currentCharacter;
    private List<Character> _characterReceptorList;
    private int _receptorIndex;
    private bool _canSelect;
    private bool _selectsAll;
    public static event EventHandler<OnCharacterReceptorSelectedEventArgs> OnCharacterSelectorStarted;
    public static event EventHandler OnCharacterSelectorCanceled;
    public static event EventHandler OnCharacterSelectorCompleted;
    public static event EventHandler OnSelectionSuccess;
    public static event EventHandler<ReceptionSelectorEventArgs> OnSelectionFailed;
    public static event EventHandler<OnCharacterReceptorSelectedEventArgs> OnCharacterReceptorSelected;
    public static event EventHandler<OnSelectsAllEventArgs> OnSelectsAll;
    public static event EventHandler<OnEnemySearchingReceptorListReadyEventArgs> OnEnemySearchingReceptorListReady;
    public class ReceptionSelectorEventArgs : EventArgs
    {
        public Character currentCharacter;
        public Attack attack;
    }
    public class OnCharacterReceptorSelectedEventArgs : EventArgs
    {
        public Character characterRecepetor;
    }
    public class OnSelectsAllEventArgs : EventArgs
    {
        public List<Character> characterReceptorList;
    }
    public class OnEnemySearchingReceptorListReadyEventArgs : EventArgs
    {
        public List<Character> characterReceptorList;
        public Attack attack;
    }

    public override void _Ready()
    {
        // Finding Nodes.
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _characterReceptorList = new List<Character>();

        // Subcribing to events.
        BattleManager.OnSelectionStarted += BattleManager_OnSelectionStarted;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterEnemy.EnemySearchingReceptorList += CharacterEnemy_EnemySearchingReceptorList;
        ItemUI.OnConfirmItem += ItemUI_OnConfirmItem;
        SkillUI.OnConfirmSkill += SkillUI_OnConfirmSkill;
        BaseAction.ActionTaken += BaseAction_ActionTaken;
    }

    public override void _Process(double delta)
    {
        if(!_canSelect)
        {
            return;
        }

        if(_selectsAll)
        {
            GD.Print("Character Receptor Selected: " + _characterReceptorList.Count + ", Current Character: " + _battleDatabase.BattleManager.GetCurrentCharacter());
            return;
        }

        int previousReceptorIndex = _receptorIndex;
        _receptorIndex = CombatCalculations.MoveTheIndex(0, _characterReceptorList.Count - 1, _receptorIndex);
        if(previousReceptorIndex != _receptorIndex)
        {
            OnCharacterReceptorSelected?.Invoke(this, new OnCharacterReceptorSelectedEventArgs{
                characterRecepetor = GetCharacterReceptor()
            });

            // Debug
            //GD.Print("Character Receptor Selected: " + _characterReceptorList[_receptorIndex] + ", Current Character: " + _battleDatabase.BattleManager.GetCurrentCharacter());
        }
    }

    private bool MeetCriteria(Character sender, Character receptor, ReceptorCriteria receptorCriteria, Attack attack)
    {
        switch(receptorCriteria)
        {
            case ReceptorCriteria.Enemy:
                return sender.DataContainer.IsEnemy != receptor.DataContainer.IsEnemy;
            case ReceptorCriteria.Ally:
                return sender.DataContainer.IsEnemy == receptor.DataContainer.IsEnemy;
            case ReceptorCriteria.IsBelowMaxHpAndAlive:
                return receptor.DataContainer.Hp < receptor.DataContainer.HpMax && receptor.DataContainer.Hp > 0;
            case ReceptorCriteria.WithoutSameModifierInflict:
                ModifierSkill modifierSkill = (ModifierSkill)attack;
                return !receptor.DataContainer.ModifierStatsInflictList.Exists(
                    (e) => e.InflictStateType == modifierSkill.ModifierStatsInflict.InflictStateType
                );
            case ReceptorCriteria.WithoutSameInflict:
                InflictSkill inflictSkill = (InflictSkill)attack;
                return receptor.DataContainer.InflictState.InflictStateType != inflictSkill.InflictState.InflictStateType; 
            case ReceptorCriteria.Dead:
                return receptor.DataContainer.Hp <= 0;
            case ReceptorCriteria.Alive:
                return receptor.DataContainer.Hp > 0;
            case ReceptorCriteria.Self:
                return sender == receptor;
            default:
                return false;
        }
    }

    public void SetupSelection(Character sender, Attack attack, bool selectsAll=false)
    {
        if(_canSelect)
        {
            return;
        }

        _receptorIndex = 0;
        _selectsAll = selectsAll;
        _characterReceptorList.Clear();

        foreach(Character character in _battleDatabase.BattleManager.CharacterTurnList)
        {
            bool hasMeetCriterias = false;
            foreach(ReceptorCriteria receptorCriteria in attack.ReceptorCriteriaList)
            {
                if(MeetCriteria(sender, character, receptorCriteria, attack))
                {
                    hasMeetCriterias = true;
                }
                else
                {
                    hasMeetCriterias = false;
                    break;
                }
            }

            if(hasMeetCriterias)
            {
                _characterReceptorList.Add(character);
            }
        }
        
        if(_characterReceptorList.Count > 0)
        {
            OnSelectionSuccess?.Invoke(this, EventArgs.Empty);
            // Send an event for knowing if you can select enemys.
        }
        else
        {
            OnSelectionFailed?.Invoke(this, new ReceptionSelectorEventArgs{
                currentCharacter = sender
            });
            CancelSelection();
        }
    }

    public void SetupSelectionPlayer(Character sender, Attack attack, bool selectsAll=false)
    {
        SetupSelection(sender, attack, selectsAll);
        if(_characterReceptorList.Count > 0)
        {
            if(selectsAll)
            {
                OnSelectsAll?.Invoke(this, new OnSelectsAllEventArgs
                {
                    characterReceptorList = _characterReceptorList
                });
            }
            else
            {
                OnCharacterSelectorStarted?.Invoke(this, new OnCharacterReceptorSelectedEventArgs{
                    characterRecepetor = _characterReceptorList[_receptorIndex]
                });

                //GD.Print("Character Receptor Selected: " + _characterReceptorList[_receptorIndex] + ", Current Character: " + _battleDatabase.BattleManager.GetCurrentCharacter());
            }

            _canSelect = true;
        }
        else
        {
            //GD.Print("No receptors possible.");
        }
    }

    private void SetupSelectionEnemy(Character sender, Attack attack)
    {
        List<ReceptorCriteria> receptorCriteriaList = new List<ReceptorCriteria>();
        
        // In some cases, we will need to change the receptorCriteria based on the skill.
        bool addedExtraCriteria = false;
        switch(attack)
        {
            case ModifierSkill:
                attack.ReceptorCriteriaList.Add(ReceptorCriteria.WithoutSameModifierInflict);
                addedExtraCriteria = true;
                break;
            case InflictSkill:
                attack.ReceptorCriteriaList.Add(ReceptorCriteria.WithoutSameInflict);
                addedExtraCriteria = true;
                break;
        }

        // The selects all will always be true, because will always need the entire list, and not only one.
        SetupSelection(sender, attack, true);

        if(addedExtraCriteria)
        {
            attack.ReceptorCriteriaList.RemoveAt(attack.ReceptorCriteriaList.Count-1);
        }

        OnEnemySearchingReceptorListReady?.Invoke(this, new OnEnemySearchingReceptorListReadyEventArgs{
            characterReceptorList = _characterReceptorList,
            attack = attack
        });

        _characterReceptorList.Clear();
    }

    public Character GetCharacterReceptor()
    {
        if(_characterReceptorList.Count > 0)
        {
            return _characterReceptorList[_receptorIndex];
        }
        else
        {
            return null;
        }
    }

    public List<Character> GetCharacterReceptorList()
    {
        if(_selectsAll)
        {
            return _characterReceptorList; 
        }

        // In this case, there is only one receptor, but we use the list, to make more dynamic the code. 
        List<Character> characterReceptorList = new List<Character>();
        characterReceptorList.Add(GetCharacterReceptor()); 
        return characterReceptorList;
    }

    public void CancelSelection()
    {
        if(!_canSelect)
        {
            return;
        }

        OnCharacterSelectorCanceled?.Invoke(this, EventArgs.Empty);
        _characterReceptorList.Clear();
        _canSelect = false;
    }

    public void CompleteSelection()
    {
        OnCharacterSelectorCompleted?.Invoke(this, EventArgs.Empty);
        _characterReceptorList.Clear();
        _canSelect = false;
    }

    private void BattleManager_OnSelectionStarted(object sender, BattleManager.OnSelectionStartedEventArgs e)
    {
        List<ReceptorCriteria> receptorCritiriaList = new List<ReceptorCriteria>();
        Attack attack = new Attack();
        switch (e.action)
        {
            case MeleeAction:
                SetupSelectionPlayer(_currentCharacter, _currentCharacter.DataContainer.MeleeAttack);
                break;
            case DefendAction:
                attack.ReceptorCriteriaList.Add(ReceptorCriteria.Self);
                SetupSelectionPlayer(_currentCharacter, attack);
                break;
            case GrupalPressionAction:
                attack.ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
                SetupSelectionPlayer(_currentCharacter, attack, true);
                break;
            default: 
                attack.ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
                SetupSelectionPlayer(_currentCharacter, attack);
                break;
        }
    }

    private void ItemUI_OnConfirmItem(object sender, ItemUI.OnConfirmItemEventArgs e)
    {
        CancelSelection();
        Attack attack = new Attack();
        attack.ReceptorCriteriaList.AddRange(e.item.ReceptorCriteriaList);
        SetupSelectionPlayer(_currentCharacter, attack, e.item.ForAllReceptors);
    }

    private void SkillUI_OnConfirmSkill(object sender, SkillUI.OnConfirmSkillEventArgs e)
    {
        SetupSelectionPlayer(_currentCharacter, e.skill, e.skill.AllReceptors);
    }

    private void CharacterEnemy_EnemySearchingReceptorList(object sender, CharacterEnemy.OnEnemySearchingReceptorEventArgs e)
    {
        // Here
        SetupSelectionEnemy(e.currentCharacter, e.attack);
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _currentCharacter = e.currentCharacter; 
    }

    private void BaseAction_ActionTaken(object sender, EventArgs e)
    {
        CompleteSelection();
    }
}