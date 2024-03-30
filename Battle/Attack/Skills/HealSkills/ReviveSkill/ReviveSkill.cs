using Godot;
using System;

public partial class ReviveSkill : Skill
{
    public override void _Ready()
    {
        ReceptorCriteriaList.Add(ReceptorCriteria.Ally);
        ReceptorCriteriaList.Add(ReceptorCriteria.Dead);
    }

    public override void UseSkill(Character character, Character receptor, out int damage)
    {
        // Based on percentage.
        damage = Mathf.RoundToInt(-(receptor.DataContainer.HpMax * ((float)Damage / 100)));
        GD.Print(Damage);
    }
}
