using Godot;
using System;

public partial class ItemUI : Node
{
    private Item _item;
    private Label _itemNameUI;
    private Label _itemQuantityUI;
    private TextureRect _itemIconUI;
    private Button _confirmItemUI;

    public static event EventHandler<OnConfirmItemEventArgs> OnConfirmItem;
    public class OnConfirmItemEventArgs : EventArgs
    {
        public Item item;
    }

    public override void _Ready()
    {
        _itemIconUI = GetNode<TextureRect>("ItemIconUI");
        _itemNameUI = GetNode<Label>("ItemNameUI");
        _itemQuantityUI = GetNode<Label>("ItemQuantityUI");
        _confirmItemUI = GetNode<Button>("ConfirmItemUI");

        _confirmItemUI.Pressed += () => {
            OnConfirmItem?.Invoke(this, new OnConfirmItemEventArgs{
                item = _item
            });
        };
    }

    public void Setup(Item item)
    {
        _item = item;
        _itemNameUI.Text = item.ItemName;
        _itemQuantityUI.Text = item.ItemQuantity.ToString();
        _itemIconUI.Texture = item.ItemImage;
    }
}