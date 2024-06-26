using Godot;
using System;
using System.Collections.Generic;

public partial class DefendAction : BaseAction
{
    public static event EventHandler<OnDefendEventArgs> OnDefend;
    public static event EventHandler<OnDefendEventArgs> OnCancelDefend;
    public static event EventHandler<OnDefendEventArgs> OnDefendActionEnded;
    public class OnDefendEventArgs : EventArgs
    {
        public Character character;
    }
    

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
	}

    private void Defend()
    {
        int armorDefense = Character.DataContainer.ArmorDefense;
        int newArmorDefense = armorDefense * 2;
        Character.DataContainer.ArmorDefense = newArmorDefense; 
        Character.DataContainer.IsDefending = true; // The event IsDefendingChanged is invoke in this function.
        OnDefend?.Invoke(this, new OnDefendEventArgs{
            character = Character
        });
        
        OnAttackState(new AttackStateEventArgs{
            damage = 0,
            isHit = false,
            current = Character,
            receptor = Character,
            baseAction = this
        });

        EndingAction();
    }

    public static void TryCancelDefend(Character characterReceptor)
    {
        if(characterReceptor.DataContainer.IsDefending)
        {
            int armorDefense = characterReceptor.DataContainer.ArmorDefense;
            int newArmorDefense = armorDefense / 2;
            characterReceptor.DataContainer.IsDefending = false;
            characterReceptor.DataContainer.ArmorDefense = newArmorDefense;

            OnCancelDefend?.Invoke(characterReceptor, new OnDefendEventArgs{
                character = characterReceptor
            });
        }
    }

    public override async void EndingAction()
    {
        Timer.Start();
        await ToSignal(Timer, Timer.SignalName.Timeout);

        OnDefendActionEnded?.Invoke(this, new OnDefendEventArgs{
            character = Character
        });

        OnActionComplete();
    }

    public override void TakeAction(List<Character> characterReceptor, Action onActionComplete)
    {
        OnActionComplete = onActionComplete;
		// Because the receptor is same as the character emisor, we don't use as a parameter.
		Defend();
        OnActionTaken();
    }

    public override string GetActionName()
    {
        return "Defend";
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(Character == e.currentCharacter)
        {
            TryCancelDefend(Character);
        }
    }
}