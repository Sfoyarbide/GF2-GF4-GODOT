using Godot;
using System;
using System.Collections.Generic;

public abstract partial class Skill : Node
{
	[Export]
	private string _skillName = "";
	[Export]
	private Texture2D _skillIcon;
	[Export]
	private int _damage = 0;
	[Export]
	private int _spCost = 0;
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

	public int Damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	public int SpCost
	{
		get { return _spCost; }
		set { _spCost = value; }
	}

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

	public abstract void UseSkill(Character character);
}

public enum SkillType
{
    Fire,
    Ice,
    Heal,
	AttackUP,
	DefendUP,
	AgilityUP,
	AttackDOWN,
	DefendDOWN,
	AgilityDOWN
} 