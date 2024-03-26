using Godot;
using System;
using System.Collections.Generic;

public partial class GrupalPressionAction : BaseAction
{
    private List<Character> _characterList = new List<Character>();
    private List<Character> _characterReceptorList = new List<Character>();

    public static event EventHandler<OnGrupalPressionEventArgs> OnGrupalPression;
    public class OnGrupalPressionEventArgs : EventArgs
    {
        public bool isEnemy; 
    }

    public override string GetActionName()
    {
        return "Grupal Pression";
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        foreach(Character character in _characterList)
        {
            if(character.DataContainer.PressionLevel != 1)
            {
                OnCannotTakeAction();
                return;
            }
        }

        InAction = true;
        _characterReceptorList = characterReceptorList;
        OnActionComplete = onActionComplete; 

        GrupalPression();
        OnActionTaken();

        EndingAction();
    }

    private void GrupalPression()
    {
        int grupalDamage = CombatCalculations.AllOutAttackDamageCalculation(_characterList, _characterReceptorList);
        foreach(Character characterReceptor in _characterReceptorList)
        {
            InflictState inflictState = new KnockDown();
            inflictState.InflictCharacter(characterReceptor);

            characterReceptor.DataContainer.Hp -= grupalDamage;

            OnAttackState(new AttackStateEventArgs{
                receptor = characterReceptor,
                damage = grupalDamage,
                isHit = true,
                inflictState = inflictState,
                baseAction = this,
                isPressionAttack = true
            });
        }

        OnGrupalPression?.Invoke(this, new OnGrupalPressionEventArgs{
            isEnemy = Character.DataContainer.IsEnemy
        });
    }

    public override void EndingAction()
    {
        InAction = false;
        OnActionComplete();
    }

    protected override void BattleManager_OnActionExecuted(object sender, BattleManager.OnActionExecutedEventArgs e)
    {
        if(e.character == Character && e.selectedAction == this)
        {
            _characterList = e.allyList;
            TakeAction(e.receptorList, e.onActionCompleted);
        }
    }
}
