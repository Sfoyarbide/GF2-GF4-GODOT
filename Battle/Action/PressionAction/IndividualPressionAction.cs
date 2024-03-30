using Godot;
using System;
using System.Collections.Generic;

public partial class IndividualPressionAction : BaseAction
{
    private Character _characterReceptor;
    public override string GetActionName()
    {
        return "Pression Action";
    }

    public override void _Ready()
    {
        base._Ready();
    }

    private void IndividualPression()
    {
        if(Character.DataContainer.PressionLevel == 1)
        {
            int damage = CombatCalculations.IndividualPressionDamageCalculation(Character, _characterReceptor);
            
            InflictState inflictState = InflictState.GetInflictStateBasedOnType(Character.DataContainer.IndividualPressionInflictStateType);
            inflictState.InflictCharacter(_characterReceptor);

            OnAttackState(new AttackStateEventArgs{
                current = Character,
                receptor = _characterReceptor,
                damage = damage,
                isHit = true,
                inflictState = inflictState,
                baseAction = this,
                isPressionAttack = true
            });

            _characterReceptor.DataContainer.Hp -= damage;
            Character.DataContainer.PressionLevel = 0;
        }
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        if(Character.DataContainer.PressionLevel != 1)
        {
            OnCannotTakeAction();
            return;
        }

        _characterReceptor = characterReceptorList[0];  
        OnActionComplete = onActionComplete;
        InAction = true;

        IndividualPression();
        OnActionTaken(); 
        EndingAction();
    }

    public override void EndingAction()
    {
        InAction = false;
        OnActionComplete();
    }
}
