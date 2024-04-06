using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Skill : Attack
{
	[Export]
	private Texture2D _skillIcon;

	[Export]
	private SkillType _skillType;

	public Texture2D SkillIcon { get { return _skillIcon; } set { _skillIcon = value; }}

	public SkillType SkillType
	{
		get { return _skillType; }
		set { _skillType = value; }
	}

    public abstract void UseSkill(Character character, Character receptor, out int damage);
}

public enum SkillType
{
	SpCost,
	HpCost,
	Heal,
	ModifierUP,
	ModifierDOWN
} 