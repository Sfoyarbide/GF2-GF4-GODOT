using Godot;
using System;

public partial class Fire : AttackSkill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
    }
}