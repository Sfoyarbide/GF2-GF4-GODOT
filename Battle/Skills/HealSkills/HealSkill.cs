using Godot;
using System;

public partial class HealSkill : Skill
{
    public override void UseSkill(Character character)
    {
        int hp = character.DataContainer.Hp; // Getting receptor's hp. 
        int healAmount = Damage; // baseDamage is refering to the healAmount in this case.
        int newHp = hp + healAmount;
        character.DataContainer.Hp = newHp;
    }
}
