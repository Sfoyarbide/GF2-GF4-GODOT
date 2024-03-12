using Godot;
using System;

public partial class KnockDown : InflictState
{
    bool _alreadyInflict;
    static public event EventHandler CurrentCharacterKnockdown;

    public KnockDown()
    {
        InflictStateType = InflictStates.KnockDown;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(e.currentCharacter != CharacterWithEffect)
        {
            return;
        }

        if(TurnRemaining <= 0)
        {
            OnEndInflitedCharacter(new InflictStateEventArgs{
                characterWithEffect = CharacterWithEffect,
                inflict = this
            });
            EndInflictState(CharacterWithEffect);
        }
        else
        {
            UseInflictEffect(CharacterWithEffect);
            TurnRemaining--;
        }
    }


    public override void UseInflictEffect(Character character)
    {
        // Invoke event for all listeners to heard that is the current character a Knockdown.
        CurrentCharacterKnockdown.Invoke(this, EventArgs.Empty);
    }
}