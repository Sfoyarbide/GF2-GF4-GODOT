using Godot;
using System;
using System.Collections.Generic;

public partial class SkillDatabase : Node
{
    private List<Skill> _skillList = new List<Skill>();
    
    public List<Skill> SkillList {get { return _skillList; }}

    public override void _Ready()
    {
        for(int x = 0; x < GetChildCount(); x++)
        {
            Skill skill = (Skill)GetChild(x);
            _skillList.Add(skill);
        }
    }

    public Skill FindSkill(string skillName)
    {
        foreach(Skill skill in _skillList)
        {
            if(skill.SkillName.Equals(skillName))
            {
                return skill;
            }
        }
        return null;
    }
}