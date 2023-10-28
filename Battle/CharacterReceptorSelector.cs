using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterReceptorSelector : Node3D
{
    private BattleDatabase _battleDatabase;
    private List<Character> _characterReceptorList;
    private int _receptorIndex;
    private bool _canSelect;
    private bool _selectsAll;
    public event EventHandler OnCharacterSelectorStarted;
    public static event EventHandler OnCharacterSelectorCanceled;
    public static event EventHandler OnCharacterSelectorCompleted;
    public static event EventHandler<OnCharacterReceptorSelectedEventArgs> OnCharacterReceptorSelected;
    public static event EventHandler<OnSelectsAllEventArgs> OnSelectsAll;
    public class OnCharacterReceptorSelectedEventArgs : EventArgs
    {
        public Character characterRecepetor;
    }
    public class OnSelectsAllEventArgs : EventArgs
    {
        public List<Character> characterReceptorList;
    }

    public override void _Ready()
    {
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _characterReceptorList = new List<Character>();
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
        _receptorIndex = MoveTheIndex(0, _characterReceptorList.Count - 1, _receptorIndex);//PlayerInputCombat.Instance.MoveTheIndex(0, selectableCharacterList.Count - 1, index);
        if(previousReceptorIndex != _receptorIndex)
        {
            OnCharacterReceptorSelected?.Invoke(this, new OnCharacterReceptorSelectedEventArgs{
                characterRecepetor = GetCharacterReceptor()
            });

            GD.Print("Character Receptor Selected: " + _characterReceptorList[_receptorIndex] + ", Current Character: " + _battleDatabase.BattleManager.GetCurrentCharacter());
        }
    }

    public void SetupSelection(bool selectsAll, bool invertCollection, bool onlySelectOnlyCharacter=false)
    {
        if(_canSelect)
        {
            return;
        }

        _selectsAll = selectsAll;

        if(!onlySelectOnlyCharacter) 
        {
            UpdateCharacterReceptorList(_selectsAll, invertCollection);
        }
        else // Selects only the character that is doing the action.
        {
            _characterReceptorList.Clear();
            _characterReceptorList.Add(_battleDatabase.BattleManager.GetCurrentCharacter());
        }

        if(!_selectsAll) // If not selects all enemys, then it means that is an indivual selection.
        {
            OnCharacterReceptorSelected?.Invoke(this, new OnCharacterReceptorSelectedEventArgs{
                characterRecepetor = GetCharacterReceptor()
            });
        }
        else // If selects all enemys, then it means is a grupal selection.
        {
            OnSelectsAll?.Invoke(this, new OnSelectsAllEventArgs
            {
                characterReceptorList = _characterReceptorList
            });
        }
        OnCharacterSelectorStarted?.Invoke(this, EventArgs.Empty);
        _canSelect = true;
    }

    private void UpdateCharacterReceptorList(bool _selectsAll, bool invertCollection) // Recheck logic.
    {
        if(_selectsAll) // If is a selection, that selects all, then it will be all the enemys selected.
        {
            if(!invertCollection)
            {
                _characterReceptorList.AddRange(_battleDatabase.BattleManager.EnemyList); // First the enemys in the collection.
            }
            else
            {
                _characterReceptorList.AddRange(_battleDatabase.BattleManager.AllyList); //  First the allys in the collection.
            }
            return;
        }

        if(!invertCollection)
        {
            _characterReceptorList.AddRange(_battleDatabase.BattleManager.EnemyList); // First the enemys in the collection.
            _characterReceptorList.AddRange(_battleDatabase.BattleManager.AllyList); // And then the allys
        }
        else
        {
            _characterReceptorList.AddRange(_battleDatabase.BattleManager.AllyList); //  First the allys in the collection.
            _characterReceptorList.AddRange(_battleDatabase.BattleManager.EnemyList); // an then the enemys.
        }
    }

    public Character GetCharacterReceptor()
    {
        if(_characterReceptorList.Count > 0)
        {
            return _characterReceptorList[_receptorIndex];
        }
        else
        {
            return _characterReceptorList[0];
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

    public int MoveTheIndex(int min, int max, int index)
    {
        int newIndexValue = index;
        if(Input.IsActionJustPressed("left"))
        {
            if(index > min)
            {
                newIndexValue--;
            }
            else
            {
                newIndexValue = max;
            }
        }
        if(Input.IsActionJustPressed("right"))
        { 
            if(index < max)
            {
                newIndexValue++;
            }
            else
            {
                newIndexValue = min;
            }
        }
        return newIndexValue;
    }
}