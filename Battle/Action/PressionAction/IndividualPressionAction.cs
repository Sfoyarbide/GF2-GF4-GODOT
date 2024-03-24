using Godot;
using System;

public partial class IndividualPressionAction : BaseAction
{
    private Character _characterReceptor;
    public override string GetActionName()
    {
        return "Pression Action";
    }

    private void IndividualPression()
    {
        if(Character.DataContainer.PressionLevel == 1)
        {
            int damage = CombatCalculations.IndividualPressionDamageCalculation(Character, _characterReceptor);
            
            InflictState inflictState = InflictState.GetInflictStateBasedOnType(Character.DataContainer.IndividualPressionInflictStateType);
            inflictState.InflictCharacter(_characterReceptor);

            _characterReceptor.DataContainer.Hp -= damage;
            Character.DataContainer.PressionLevel = 0;

            OnAttackState(new AttackStateEventArgs{
                current = Character,
                receptor = _characterReceptor,
                damage = damage,
                isHit = true,
                inflictState = inflictState,
                baseAction = this,
                isPressionAttack = true
            });
        }
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        if(Character.DataContainer.PressionLevel != 1)
        {
            OnCannotTakeAction();
            return;
        }

        _characterReceptor = characterReceptor;  
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