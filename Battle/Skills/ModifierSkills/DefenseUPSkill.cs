using Godot;
using System;

public partial class DefenseUPSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ModifierStatsInflict = new DefenseUP();
    }
}