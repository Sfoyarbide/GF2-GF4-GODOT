using Godot;
using System;

public partial class AttackSkill : Skill
{
    public override void UseSkill(Character character)
    {
        int hp = character.DataContainer.Hp; // Getting receptor's hp. 
        int armorDefense = character.DataContainer.ArmorDefense + character.DataContainer.Co;
        int damage;

        damage = CombatCalculations.CalculateDamage(Damage, armorDefense);

        if(damage <= 0)
        {
            // Logic code to do.
            return;
        }
        
        GD.Print("Action: SkillAction, Skill: " + SkillName + ", Damage: " + damage + ", Receptor Hp: " + character.DataContainer.Hp + ", Receptor ID: " + character);
        character.DataContainer.Hp -= Damage;
    }
}