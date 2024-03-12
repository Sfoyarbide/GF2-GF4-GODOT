using Godot;
using System;

public partial class Poison : InflictState
{
    private bool _isCharacterTurn;

    public Poison()
    {
        InflictStateType = InflictStates.Poison;
        BattleManager.OnTurnEnd += BattleManager_OnTurnEnd;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    public override void UseInflictEffect(Character character)
    {
        character.DataContainer.Hp -= character.DataContainer.HpMax * 2 / 10;
    }

    private void BattleManager_OnTurnEnd(object sender, EventArgs e)
    {
        if(!_isCharacterTurn)
        {
            return;
        }

        UseInflictEffect(CharacterWithEffect);
        TurnRemaining--;
        if(TurnRemaining == 0)
        {
            OnEndInflitedCharacter(new InflictStateEventArgs{
                characterWithEffect = CharacterWithEffect,
                inflict = this
            });
            EndInflictState(CharacterWithEffect);
        }
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(e.currentCharacter == CharacterWithEffect)
        {
            _isCharacterTurn = true;
        }
    }
}