using Godot;
using System;

public partial class MeleeAttack : Attack
{
    public override void _Ready()
    {
        AttackType = AttackTypes.Strike;
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
        ReceptorCriteriaList.Add(ReceptorCriteria.Alive);
    }
}