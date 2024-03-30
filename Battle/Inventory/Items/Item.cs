using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Item : Node
{
    [Export]
    private string _itemName;
    [Export]
    private Texture2D _itemImage;
    [Export]
    private int _itemQuantity;
    [Export]
    private bool _forAllReceptors;
    private List<ReceptorCriteria> _receptorCriteriaList = new List<ReceptorCriteria>();
    private Inventory _inventory;

    public static event EventHandler<ItemUsedEventArgs> ItemUsed; 
    public class ItemUsedEventArgs : EventArgs
    {
        public int damage;
        public Character receptor;
    }

    public string ItemName {get {return _itemName;} set {_itemName = value;}}
    public Texture2D ItemImage {get {return _itemImage; } set {_itemImage = value;}}
    public int ItemQuantity {get {return _itemQuantity; } set {_itemQuantity = value;}}
    public List<ReceptorCriteria> ReceptorCriteriaList {get { return _receptorCriteriaList; }}
    public bool ForAllReceptors {get { return _forAllReceptors; }}
    public Inventory Inventory {get {return _inventory; } set {_inventory = value;}}

    public abstract void UseItem(Character character);

    public void SubtractItem()
    {
        Inventory.SubtractItem(this, 1);
    }

    protected void OnItemUsed(ItemUsedEventArgs e)
    {
        ItemUsed?.Invoke(this, e);
    }
}