using Godot;
using System;

public partial class HealSkill : Skill
{
    public override void UseSkill(Character character, Character receptor, out int healAmount)
    {
        int hp = receptor.DataContainer.Hp; // Getting receptor's hp. 
        healAmount = (int)Damage;
        int newHp = hp + healAmount;
        receptor.DataContainer.Hp = newHp;
    }
}
