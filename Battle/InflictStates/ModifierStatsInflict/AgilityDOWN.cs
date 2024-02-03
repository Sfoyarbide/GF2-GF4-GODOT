using Godot;
using System;

public partial class AgilityDOWN : ModifierStatsInflict
{

    public AgilityDOWN()
    {
        InflictStateType = InflictStates.AttackDOWN;
        OpposedModifierStatsInflictType = InflictStates.AgilityUP;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    public override void UseInflictEffect(Character character)
    {
        CharacterWithEffect = character;
        character.DataContainer.Ag /= 2;
        character.DataContainer.Lu /= 2;
        ModifierStatsEventArgs modifierStatsEventArgs = new ModifierStatsEventArgs{
            characterWithEffect = character,
            modifierStatsInflict = this
        };
        OnModifierStatsInflicted(modifierStatsEventArgs);
    }

    protected override void ReturnNormalValues(Character character)
    {
        CharacterWithEffect.DataContainer.Ag *= 2;
        CharacterWithEffect.DataContainer.Lu *= 2;
    }

    // NOT WORKING.
    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        if(e.currentCharacter == CharacterWithEffect)
        {
            TurnRemaining--; 
            if(TurnRemaining == 0)
            {
                ModifierStatsEventArgs modifierStatsEventArgs = new ModifierStatsEventArgs{
                    characterWithEffect = CharacterWithEffect,
                    modifierStatsInflict = this
                };
                OnModifierStatsEnded(modifierStatsEventArgs);
                EndInflictState(CharacterWithEffect);
            }
        }
    }
}