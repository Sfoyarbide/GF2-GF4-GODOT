using Godot;
using System;

public partial class Heal : HealSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
    }
}
