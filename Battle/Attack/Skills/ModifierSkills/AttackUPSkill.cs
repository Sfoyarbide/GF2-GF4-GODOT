using Godot;
using System;

public partial class AttackUPSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ModifierStatsInflict = new AttackUP();
    }
}