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
        base._Ready();
        Item.ItemUsed += Item_ItemUsed;
        ItemUI.OnConfirmItem += ItemUI_OnConfirmItem;
    }


    public override void EndingAction()
    {
        InAction = false;
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

        if(!_currentItem.ForAllReceptors)
        {
            _currentItem.UseItem(_characterReceptorList[0]);
        }
        else
        {
            foreach(Character character in _characterReceptorList)
            {
                _currentItem.UseItem(character);
            }
        }
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        _characterReceptorList = characterReceptorList;
        OnActionComplete = onActionComplete;

        InAction = true;

        UseItem();
        OnActionTaken();
    }

    private void ItemUI_OnConfirmItem(object sender, ItemUI.OnConfirmItemEventArgs e)
    {
        _currentItem = e.item;
    }

    private void Item_ItemUsed(object sender, Item.ItemUsedEventArgs e)
    {
        if(!InAction)
        {
            return;
        }

        int finalDamage = e.damage;
        if(e.damage < 0)
        {
            finalDamage = -finalDamage;
        }

        OnAttackState(new AttackStateEventArgs{
            current = Character,
            receptor = e.receptor,
            damage = finalDamage,
            isHit = true,
            baseAction = this
        });

        e.receptor.DataContainer.Hp -= e.damage;
        _currentItem.SubtractItem();

        EndingAction();
    }
}