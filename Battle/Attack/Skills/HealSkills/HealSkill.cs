using Godot;
using System;

public partial class HealSkill : Skill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ReceptorCriteriaList.Add(ReceptorCriteria.IsBelowMaxHp);
    }

    public override void UseSkill(Character character, Character receptor, out int healAmount)
    {
        int hp = receptor.DataContainer.Hp; // Getting receptor's hp. 
        healAmount = (int)Damage;
        receptor.DataContainer.Hp += healAmount;
    }
}
