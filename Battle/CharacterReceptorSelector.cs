using Godot;
using System;
using System.Collections.Generic;

public enum ReceptorCriteria
{
    Enemy,
    Ally,
    Self
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
    public static event EventHandler<OnCharacterReceptorSelectedEventArgs> OnCharacterReceptorSelected;
    public static event EventHandler<OnSelectsAllEventArgs> OnSelectsAll;
    public static event EventHandler<OnAISearchingReceptorListReadyEventArgs> OnAISearchingReceptorListReady;
    public class OnCharacterReceptorSelectedEventArgs : EventArgs
    {
        public Character characterRecepetor;
    }
    public class OnSelectsAllEventArgs : EventArgs
    {
        public List<Character> characterReceptorList;
    }
    public class OnAISearchingReceptorListReadyEventArgs : EventArgs
    {
        public List<Character> EnemyReceptorList;
    }

    public override void _Ready()
    {
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _battleDatabase.BattleManager.OnSelectionStarted += BattleManager_OnSelectionStarted;
        _characterReceptorList = new List<Character>();
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterEnemy.EnemySearchingReceptorList += CharacterEnemy_EnemySearchingReceptorList;
        ItemUI.OnConfirmItem += ItemUI_OnConfirmItem;
        SkillUI.OnConfirmSkill += SkillUI_OnConfirmSkill;
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

    public void SetupSelection(Character sender, List<ReceptorCriteria> receptorCriteriaList, bool selectsAll=false)
    {
        if(_canSelect)
        {
            return;
        }

        _receptorIndex = 0;

        _characterReceptorList.Clear();

        foreach(Character character in _battleDatabase.BattleManager.CharacterTurnList)
        {
            bool hasMeetCriterias = false;
            foreach(ReceptorCriteria receptorCriteria in receptorCriteriaList)
            {
                if(MeetCriteria(sender, character, receptorCriteria))
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
    }

    public void SetupSelectionPlayer(Character sender, List<ReceptorCriteria> receptorCriteriaList, bool selectsAll=false)
    {
        SetupSelection(sender, receptorCriteriaList, selectsAll);
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

                GD.Print("Character Receptor Selected: " + _characterReceptorList[_receptorIndex] + ", Current Character: " + _battleDatabase.BattleManager.GetCurrentCharacter());
            }

            _canSelect = true;
        }
        else
        {
            GD.Print("No receptors possible.");
        }
    }

    public void SetupSelectionAI(Character sender, List<ReceptorCriteria> receptorCriteriaList, bool selectsAll=false)
    {
        SetupSelection(sender, receptorCriteriaList, selectsAll);
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
        return _characterReceptorList;
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
        switch (e.action)
        {
            case MeleeAction:
                receptorCritiriaList.Add(ReceptorCriteria.Enemy);
                SetupSelectionPlayer(_currentCharacter, receptorCritiriaList);
                break;
            case DefendAction:
                receptorCritiriaList.Add(ReceptorCriteria.Self);
                SetupSelectionPlayer(_currentCharacter, receptorCritiriaList);
                break;
        }
    }

    private bool MeetCriteria(Character sender, Character receptor, ReceptorCriteria receptorCriteria)
    {
        switch(receptorCriteria)
        {
            case ReceptorCriteria.Enemy:
                return sender.DataContainer.IsEnemy != receptor.DataContainer.IsEnemy;
            case ReceptorCriteria.Ally:
                return sender.DataContainer.IsEnemy == sender.DataContainer.IsEnemy;
            case ReceptorCriteria.Self:
                return sender == receptor;
            default:
                return false;
        }
    }

    private void ItemUI_OnConfirmItem(object sender, ItemUI.OnConfirmItemEventArgs e)
    {
        SetupSelectionPlayer(_currentCharacter, e.item.ReceptorCriteriaList, e.item.ForAllReceptors);
    }

    private void SkillUI_OnConfirmSkill(object sender, SkillUI.OnConfirmSkillEventArgs e)
    {
        SetupSelectionPlayer(_currentCharacter, e.skill.ReceptorCriteriaList, e.skill.IsAllReceiveDamage);
    }

    private void CharacterEnemy_EnemySearchingReceptorList(object sender, CharacterEnemy.OnEnemySearchingReceptorEventArgs e)
    {
        SetupSelectionAI(_currentCharacter, e.receptorCriteriaList);
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _currentCharacter = e.currentCharacter; 
    }
}