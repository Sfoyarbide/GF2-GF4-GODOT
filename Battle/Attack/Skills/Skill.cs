using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Skill : Attack
{
	[Export]
	private string _skillName = "";
	[Export]
	private Texture2D _skillIcon;
	[Export]
	private bool _isAllReceiveDamage = false;

	private List<ReceptorCriteria> receptorCriteriaList = new List<ReceptorCriteria>();
	[Export]
	private SkillType _skillType;

	public string SkillName
	{
		get { return _skillName; }
		set { _skillName = value; }
	}

	public Texture2D SkillIcon { get { return _skillIcon; } set { _skillIcon = value; }}

	public bool IsAllReceiveDamage
	{
		get { return _isAllReceiveDamage; }
		set { _isAllReceiveDamage = value; }
	}

	public List<ReceptorCriteria> ReceptorCriteriaList { get { return receptorCriteriaList; }set { receptorCriteriaList = value; } }

	public SkillType SkillType
	{
		get { return _skillType; }
		set { _skillType = value; }
	}

    public override string ToString()
    {
        return _skillName;
    }

    public abstract void UseSkill(Character character, Character receptor, out int damage);
}

public enum SkillType
{
	SpCost,
	HpCost,
	Heal,
	AttackUP,
	DefendUP,
	AgilityUP,
	AttackDOWN,
	DefendDOWN,
	AgilityDOWN
} 