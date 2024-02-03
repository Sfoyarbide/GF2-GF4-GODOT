using Godot;
using System;
using System.Collections.Generic;

public partial class SkillAction : BaseAction
{
    private Skill _currentSkill;

    private List<Character> _characterReceptorList = new List<Character>();
    public static event EventHandler<OnSkillEventArgs> OnSkillCast;
    public static event EventHandler<OnSkillEventArgs> OnSkillEnd;
    public class OnSkillEventArgs : EventArgs
    {
        public Character character;
        public Skill skill;
    }

    public Skill CurrentSkill { get {return _currentSkill; }}

    public override void _Ready()
    {
        base._Ready();
        SkillUI.OnConfirmSkill += SkillUI_OnConfirmSkill;
    }

    private bool IsSkill(Character characterReceptor, Skill skill)
    {  
        if(Character.DataContainer.Sp < skill.SpCost)
        {
            return false;
        }

        Character.DataContainer.Sp -= skill.SpCost;

        switch(skill)
        {
            case HealSkill healSkill:
                skill.UseSkill(characterReceptor);
                return true;
            case ModifierSkill modifierSkill:
                skill.UseSkill(characterReceptor);
                return true;
            default:
                break;
        }

        int maCharacter = Character.DataContainer.Ma;
        int agCharacter = Character.DataContainer.Ag;
        int characterChance = maCharacter + agCharacter;

        int agCharacterReceptor = characterReceptor.DataContainer.Ag;

        int dice = GD.RandRange(0,10);

        if(CombatCalculations.CheckIsHit(characterChance, agCharacterReceptor, dice))
        {
            skill.UseSkill(characterReceptor);
            return true;
        }
        else
        {
            GD.Print("Action: SkillAction, Skill: " + skill + ", Damage: " + "Not hit" + ", Receptor Hp: " + characterReceptor.DataContainer.Hp  + ", Receptor ID: " + characterReceptor);
            return false;
        }
    }

    private void InvokeSkillToCharacterReceptorList(List<Character> characterReceptorList, Skill skill)
    {
        if(skill.IsAllReceiveDamage)
        {
            foreach(Character characterReceptor in characterReceptorList)
            {
                bool isSkill = IsSkill(characterReceptor, skill); 
            }
        }
        else
        {
            bool isSkill = IsSkill(characterReceptorList[0], skill); 
        }
    }

    private void ExecuteSkill(Skill skill)
    {
        InvokeSkillToCharacterReceptorList(_characterReceptorList, skill);

        OnSkillCast?.Invoke(this, new OnSkillEventArgs{
            character = Character,
            skill = skill
        });

        EndingAction();
    }

    public override async void EndingAction()
    {
        Timer.Start();
        await ToSignal(Timer, Timer.SignalName.Timeout);

        OnSkillEnd?.Invoke(this, new OnSkillEventArgs{
            character = Character
        });

        OnActionComplete();
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        _characterReceptorList.Add(characterReceptor);
        OnActionComplete = onActionComplete;
        ExecuteSkill(_currentSkill);
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        _characterReceptorList = characterReceptorList;
        OnActionComplete = onActionComplete;
        ExecuteSkill(_currentSkill);
    }

    private void SkillUI_OnConfirmSkill(object sender, SkillUI.OnConfirmSkillEventArgs e)
    {
        _currentSkill = e.skill;
    }

    public override string GetActionName()
    {
        return "Skill";
    }
}
