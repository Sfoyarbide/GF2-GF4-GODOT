using Godot;
using System;

public partial class AttackSkill : Skill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
    }

    public override void UseSkill(Character character, Character receptor, out int damage)
    {
        // The damage is done outside this class.
        damage = CombatCalculations.DamageCalculation(character, receptor, this);   
    }
}