using Godot;
using System;

public partial class HealSkill : Skill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ReceptorCriteriaList.Add(ReceptorCriteria.IsBelowMaxHpAndAlive);
    }

    public override void UseSkill(Character character, Character receptor, out int healAmount)
    {
        healAmount = -(int)Damage;
    }
}
