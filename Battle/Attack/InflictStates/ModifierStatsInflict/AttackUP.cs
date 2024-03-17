using Godot;
using System;

public partial class AttackUP : ModifierStatsInflict
{
    public AttackUP()
    {
        InflictStateType = InflictStates.AttackUP;
        OpposedModifierStatsInflictType = InflictStates.AttackDOWN;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
    }

    public override void UseInflictEffect(Character character)
    {
        CharacterWithEffect = character;
        character.DataContainer.AttackModifier += 0.5f;
        ModifierStatsEventArgs modifierStatsEventArgs = new ModifierStatsEventArgs{
            characterWithEffect = character,
            modifierStatsInflict = this
        };
        OnModifierStatsInflicted(modifierStatsEventArgs);
    }

    protected override void ReturnNormalValues(Character character)
    {
        character.DataContainer.AttackModifier = 1;
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