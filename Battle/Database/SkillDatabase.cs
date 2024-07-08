using Godot;
using System;
using System.Collections.Generic;

public partial class SkillDatabase : Node
{
    private List<Skill> _skillList = new List<Skill>();
    
    public List<Skill> SkillList {get { return _skillList; }}

    public override void _Ready()
    {
        // Getting skills.
        for(int x = 0; x < GetChildCount(); x++)
        {
            Skill skill = (Skill)GetChild(x);
            _skillList.Add(skill);
        }

        //Subcribing to events.
        BattleStarter.OnBattleCharacterSetupFinished += BattleStarter_OnBattleCharacterSetupFinished;
    }

    public Skill FindSkill(string skillName)
    {
        foreach(Skill skill in _skillList)
        {
            if(skill.AttackName.Equals(skillName))
            {
                return skill;
            }
        }
        return null;
    }

    private void BattleStarter_OnBattleCharacterSetupFinished(object sender, BattleStarter.OnBattleCharacterSetupFinishedEventArgs e)
    {
        foreach(Character character in e.characterTurnList)
        {
            character.DataContainer.SkillList.Clear();
            foreach(string skillName in character.DataContainer.SkillNameList)
            {
                Skill skill = FindSkill(skillName);
                character.DataContainer.SkillList.Add(skill);
            }  
        }
    }
}