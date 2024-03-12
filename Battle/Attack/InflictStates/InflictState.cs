using Godot;
using System;

public enum InflictStates
{
	KnockDown,
    Poison,
	AttackUP,
	DefendUP,
	AgilityUP,
	AttackDOWN,
	DefendDOWN,
	AgilityDOWN
}

public abstract partial class InflictState
{
	public static event EventHandler<InflictStateEventArgs> InflictedCharacter;
	public static event EventHandler<InflictStateEventArgs> EndInflictedCharacter;
	public class InflictStateEventArgs : EventArgs
	{
		public Character characterWithEffect;
		public InflictState inflict;
	}

	private Character _characterWithEffect;

    public Character CharacterWithEffect {get { return _characterWithEffect; } set {_characterWithEffect = value;}}

	private InflictStates _inflictStateType;
    private int _turnRemaining = 1;

	public InflictStates InflictStateType {get {return _inflictStateType;} protected set {_inflictStateType = value; }}
    public int TurnRemaining {get {return _turnRemaining;} set {_turnRemaining = value;}}

	public void InflictCharacter(Character character)
	{
		if(character.DataContainer.InflictState != null)
		{
			// If it is the same inflict state, it will not inflict, beacause it is already inflicted.
			if(character.DataContainer.InflictState.InflictStateType == InflictStateType)
			{
				return;
			}
		}

		OnInflitedCharacter(new InflictStateEventArgs{
			characterWithEffect = CharacterWithEffect,
			inflict = this
		});
		character.DataContainer.InflictState = this;
		_characterWithEffect = character;
	}

	public abstract void UseInflictEffect(Character character);

	public virtual void EndInflictState(Character character)
	{
		character.DataContainer.InflictState = null;
	}

	protected void OnInflitedCharacter(InflictStateEventArgs e)
	{
		InflictedCharacter?.Invoke(this, e);
	}

	protected void OnEndInflitedCharacter(InflictStateEventArgs e)
	{
		EndInflictedCharacter?.Invoke(this, e);
	}
}