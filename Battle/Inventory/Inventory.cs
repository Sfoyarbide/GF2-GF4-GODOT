using Godot;
using System;
using System.Collections.Generic;

public partial class Inventory : Node
{
    private const int MAX_ITEM_QUANTITY = 99;
    private List<Item> _inventory = new List<Item>();
    public List<Item> ItemInventory {get {return _inventory;} private set {}}

    public override void _Ready()
    {
        for(int x = 0; x < GetChildCount(); x++)
        {
            Item item = (Item)GetChild(x);
            if(FindItem(item) == null)
            {
                AddItem(item);
            }
        }
    }

    public int FindItemIndex(Item requestItem)
    {
        // Search in each entry.
        int index = 0;
        foreach(Item item in _inventory)
        {
            // See if there is a match.
            if(item.ItemName.Equals(requestItem.ItemName))
            {
                return index;
            }
            index++;
        }
        return -1;
    }

    public Item FindItem(Item requestItem)
    {
        // Search in each entry.
        foreach(Item item in _inventory)
        {
            // See if there is a match.
            if(item.ItemName.Equals(requestItem.ItemName))
            {
                return item;
            }
        }
        return null;
    }

    public void RemoveItem(Item requestItem)
    {
        int matchItemIndex = FindItemIndex(requestItem);

        // Did not find a match.
        if(matchItemIndex.Equals(-1))
        {
            return;
        }

        _inventory.RemoveAt(matchItemIndex);
    }

    public void AddItem(Item newItem)
    {
        Item matchItem = FindItem(newItem);
        if(matchItem == null)
        {
            _inventory.Add(newItem);
            newItem.Inventory = this;
        }
        else
        {
            matchItem.ItemQuantity += newItem.ItemQuantity;

            if(matchItem.ItemQuantity >= MAX_ITEM_QUANTITY)
            {
                matchItem.ItemQuantity = MAX_ITEM_QUANTITY;
            }
        }
    }

    public void SubtractItem(Item newItem)
    {
        Item matchItem = FindItem(newItem);
        if(matchItem == null)
        {
            return;
        }

        if(ItemHasCertainQuantity(matchItem, newItem.ItemQuantity))
        {
            matchItem.ItemQuantity -= newItem.ItemQuantity;

            if(matchItem.ItemQuantity == 0)
            {
                _inventory.RemoveAt(FindItemIndex(matchItem));
                matchItem.QueueFree();
            }
        }
    }

    public void SubtractItem(Item item, int quantity)
    {
        Item matchItem = FindItem(item);
        if(matchItem == null)
        {
            return;
        }

        if(ItemHasCertainQuantity(matchItem, item.ItemQuantity))
        {
            matchItem.ItemQuantity -= quantity;

            if(matchItem.ItemQuantity == 0)
            {
                _inventory.RemoveAt(FindItemIndex(matchItem));
                matchItem.QueueFree();
            }
        }
    }

    public bool ItemHasCertainQuantity(Item requestItem, int quantity)
    {
        Item matchItem = FindItem(requestItem);
        if(matchItem == null)
        {
            return false;
        }

        if(matchItem.ItemQuantity - quantity >= 0)
        {
            return true;
        }
         
        return false;
    }
}