using Godot;
using System;

public abstract partial class ModifierStatsInflict : InflictState
{
    public static event EventHandler<ModifierStatsEventArgs> ModifierStatsInflicted;
    public static event EventHandler<ModifierStatsEventArgs> ModifierStatsEnded;
    public class ModifierStatsEventArgs : EventArgs
    {
        public Character characterWithEffect;
        public ModifierStatsInflict modifierStatsInflict;
    }

    private InflictStates _opposedModifierStatsInflictType;

    public InflictStates OpposedModifierStatsInflictType {get { return _opposedModifierStatsInflictType; } set { _opposedModifierStatsInflictType = value;}}

    public ModifierStatsInflict()
    {
        TurnRemaining = 3;
    }

    public override void EndInflictState(Character character)
	{
	 	ModifierStatsInflict modifierStatsInflict = character.DataContainer.ModifierStatsInflictList.Find(x => x.InflictStateType == InflictStateType);
        character.DataContainer.ModifierStatsInflictList.Remove(modifierStatsInflict);
        ReturnNormalValues(character);
    }

    protected abstract void ReturnNormalValues(Character character);

    protected void OnModifierStatsInflicted(ModifierStatsEventArgs e)
    {
        ModifierStatsInflicted?.Invoke(this, e);
    }

    protected void OnModifierStatsEnded(ModifierStatsEventArgs e)
    {
        ModifierStatsEnded?.Invoke(this, e);
    }
}