using Godot;
using System;

public partial class AttackDOWNSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
        ModifierStatsInflict = new AttackDOWN();
    }
}
