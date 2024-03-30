using Godot;
using System;

public partial class ReviveItem : Item
{
    [Export]
    private int _percentageHealed;

    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ReceptorCriteriaList.Add(ReceptorCriteria.Dead);
    }

    public override void UseItem(Character character)
    {
        int damage = Mathf.RoundToInt(-(character.DataContainer.HpMax * ((float)_percentageHealed / 100)));
        OnItemUsed(new ItemUsedEventArgs{
            receptor = character,
            damage = damage
        });
    }
}