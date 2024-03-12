using Godot;
using System;

public partial class DefenseDOWNSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
        ModifierStatsInflict = new DefenseDOWN();
    }
}
