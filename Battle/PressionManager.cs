using Godot;
using System;
using System.Collections.Generic;

public partial class PressionManager : Node
{
    public override void _Ready()
    {
        BaseAction.AttackState += BaseAction_AttackState;
    }

    private void BaseAction_AttackState(object sender, BaseAction.AttackStateEventArgs e)
    {
        PressionCheck(e);
    }

    private void PressionCheck(BaseAction.AttackStateEventArgs attackState)
    {
        if(!attackState.isHit)
        {
            return;
        }

        if(attackState.isPressionAttack)
        {
            return;
        }

        Character character = attackState.current;
        Character receptor = attackState.receptor;

        if(receptor.DataContainer.PressionLevelModifier == PressionLevelModifier.ReceiveDamage)
        {
            if(attackState.attack.AttackType != AttackTypes.Heal)
            {
                receptor.DataContainer.PressionLevel += 0.25f;
            }
        }

        switch(character.DataContainer.PressionLevelModifier)
        {
            case PressionLevelModifier.Attack:
                if(attackState.attack.AttackType != AttackTypes.Heal)
                {
                    character.DataContainer.PressionLevel += 0.25f;
                    return;
                }
                character.DataContainer.PressionLevel += 0.10f;
                break;
            case PressionLevelModifier.Healer:
                if(attackState.attack.AttackType == AttackTypes.Heal)
                {
                    character.DataContainer.PressionLevel += 0.25f;
                    return;
                }
                character.DataContainer.PressionLevel += 0.10f;
                break;
        }
    }
}