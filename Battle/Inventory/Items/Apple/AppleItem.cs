using Godot;
using System;

public partial class AppleItem : Item
{
    private int _healingValue = 25;

    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ItemName = "Apple";
    }

    public override void UseItem(Character character)
    {
        character.DataContainer.Hp += 25;
    }
}