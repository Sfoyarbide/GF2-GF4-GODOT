using Godot;
using System;

public partial class AgilityUPSkill : ModifierSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ModifierStatsInflict = new AgilityUP();
    }
}
