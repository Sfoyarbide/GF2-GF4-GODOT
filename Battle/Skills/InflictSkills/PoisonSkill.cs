using Godot;
using System;

public partial class PoisonSkill : Skill
{
    private InflictState _poisonInflictState;
    public override void _Ready()
    {
        _poisonInflictState = new Poison();
        ReceptorCriteriaList.Add(ReceptorCriteria.Enemy);
    }

    public override void UseSkill(Character character)
    {
        _poisonInflictState.InflictCharacter(character);
    }
}