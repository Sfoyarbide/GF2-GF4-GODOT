using Godot;
using System;
using System.Reflection;

public partial class AttackSkill : Skill
{
    public override void UseSkill(Character character, Character receptor, out int damage)
    {
        damage = CombatCalculations.DamageCalculation(character, receptor, this);
        
        GD.Print("Action: SkillAction, Skill: " + SkillName + ", Damage: " + damage + ", Receptor Hp: " + character.DataContainer.Hp + ", Receptor ID: " + character);
        receptor.DataContainer.Hp -= (int)Damage;
    }
}