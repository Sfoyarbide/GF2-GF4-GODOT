using Godot;
using System;
using System.Reflection.Metadata.Ecma335;

public partial class AgilityUP : ModifierStatsInflict
{
    public AgilityUP()
    {
        InflictStateType = InflictStates.AgilityUP;
        OpposedModifierStatsInflictType = InflictStates.AgilityDOWN;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    public override void UseInflictEffect(Character character)
    {
        CharacterWithEffect = character;
        character.DataContainer.AccuracyModifier += 0.5f;
        ModifierStatsEventArgs modifierStatsEventArgs = new ModifierStatsEventArgs{
            characterWithEffect = character,
            modifierStatsInflict = this
        };
        OnModifierStatsInflicted(modifierStatsEventArgs);
    }

    protected override void ReturnNormalValues(Character character)
    {
        character.DataContainer.AccuracyModifier = 1;
    }


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
