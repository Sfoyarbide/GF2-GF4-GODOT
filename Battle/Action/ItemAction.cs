using Godot;
using System;
using System.Collections.Generic;


public partial class ItemAction : BaseAction
{
    private List<Character> _characterReceptorList = new List<Character>();
    private Item _currentItem;

    public static event EventHandler<OnUsedItemEventArgs> OnUsedItemStarted;
    public static event EventHandler OnUsedItem; 
    public class OnUsedItemEventArgs : EventArgs
    {
        public Character character; 
    }

    public Item CurrentItem { get { return _currentItem; } }

    public override void _Ready()
    {
        ItemUI.OnConfirmItem += ItemUI_OnConfirmItem;
    }

    public override void EndingAction()
    {
        _characterReceptorList.Clear();
        OnActionComplete();
    }

    public override string GetActionName()
    {
        return "Item";
    }

    private void UseItem()
    {
        OnUsedItemStarted?.Invoke(this, new OnUsedItemEventArgs{
            character = Character
        });

        foreach(Character character in _characterReceptorList)
        {
            _currentItem.UseItem(character);
        }

        _currentItem.SubtractItem();

        EndingAction();
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        _characterReceptorList = characterReceptorList;

        OnActionComplete = onActionComplete;
        UseItem();
        OnActionComplete();
        OnActionTaken();
    }

    private void ItemUI_OnConfirmItem(object sender, ItemUI.OnConfirmItemEventArgs e)
    {
        _currentItem = e.item;
    }
}