using Godot;
using System;

public partial class Ice : AttackSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
    }
}