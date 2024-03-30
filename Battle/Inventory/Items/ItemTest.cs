using Godot;
using System;

public partial class ItemTest : Item
{
    [Export]
    private string _itemName;
    [Export]
    private ReceptorCriteria receptorCriteria1;

    public override void _Ready()
    {
        ReceptorCriteriaList.Add(receptorCriteria1);
        ItemName = _itemName;
    }

    public override void UseItem(Character character)
    {
        //character.DataContainer.Hp += 15;
        OnItemUsed(new ItemUsedEventArgs{
            damage = 15
        });
    }

}
