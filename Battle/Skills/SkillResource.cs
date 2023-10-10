using Godot;
using System;

public partial class SkillResource : Resource
{
	private string _skillName = "";
	private int _damage = 0;
	private int _spCost = 0;
	private bool _isAllReceiveDamage = false;
	private SkillType _skillType = SkillType.Fire;

	[Export]
	public string SkillName
	{
		get { return _skillName; }
		set { _skillName = value; }
	}

	[Export]
	public int Damage
	{
		get { return _damage; }
		set { _damage = value; }
	}

	[Export]
	public int SpCost
	{
		get { return _spCost; }
		set { _spCost = value; }
	}

	[Export]
	public bool IsAllReceiveDamage
	{
		get { return _isAllReceiveDamage; }
		set { _isAllReceiveDamage = value; }
	}

	[Export]
	public SkillType SkillType
	{
		get { return _skillType; }
		set { _skillType = value; }
	}

    public override string ToString()
    {
        return _skillName;
    }
}

public enum SkillType
{
    Fire,
    Ice,
    Heal
} 