using Godot;
using System;

public partial class InventoryUI : Control
{
    private Inventory _inventory;
    [Export]
    private PackedScene _itemUIScene;
    private VBoxContainer _itemContainerUI;

    public override void _Ready()
    {
        BattleDatabase battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _inventory = battleDatabase.Inventory;
        battleDatabase.BattleManager.OnItemSelectionStarted += BattleManager_OnItemSelectionStarted;
        ItemUI.OnConfirmItem += ItemUI_OnConfirmItem;

        if(_itemUIScene == null)
        {
            GD.PrintErr("Item UI Scene has not been assigned. Plese add it in Inventory UI.");
        }

        _itemContainerUI = GetNode<VBoxContainer>("Panel/ScrollContainer/VBoxContainer");

        ShowInventoryUI();
    }

    public void ShowInventoryUI()
    {
        ClearInventoryUI();
        // Only do this if has been changed. Add this.
        foreach(Item item in _inventory.ItemInventory)
        {
            ItemUI itemUI = (ItemUI)_itemUIScene.Instantiate();
            _itemContainerUI.AddChild(itemUI);
            itemUI.Setup(item);
        }
    }

    public void ClearInventoryUI()
    {
        for(int x = 0; x < _itemContainerUI.GetChildCount(); x++)
        {
            _itemContainerUI.GetChild(x).QueueFree();
        }
    }

    private void BattleManager_OnItemSelectionStarted(object sender, EventArgs e)
    {
        Show();
        ShowInventoryUI();
    }

    private void ItemUI_OnConfirmItem(object sender, ItemUI.OnConfirmItemEventArgs e)
    {
        Hide();
    }
}