using Godot;
using System;

public partial class AgilityDOWNSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
        ModifierStatsInflict = new AgilityDOWN();
    }
}