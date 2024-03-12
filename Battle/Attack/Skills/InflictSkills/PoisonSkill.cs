using Godot;
using System;

public partial class PoisonSkill : InflictSkill
{
    public override void _Ready()
    {
        InflictState = new Poison();
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
    }
}