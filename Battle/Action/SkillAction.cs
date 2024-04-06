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

    public Skill CurrentSkill { get {return _currentSkill; } set {_currentSkill = value;}}

    public override void _Ready()
    {
        base._Ready();
        SkillUI.OnConfirmSkill += SkillUI_OnConfirmSkill;
    }

    private void Skill(Character characterReceptor, Skill skill)
    {  
        bool isHit = CombatCalculations.IsHitCalculation(Character, characterReceptor);
        Character.DataContainer.Sp -= skill.Cost;
        
        int damage = 0;
        switch(skill)
        {
            case HealSkill healSkill:
                isHit = true;
                //skill.UseSkill(Character, characterReceptor, out damage);
                break;
            case ModifierSkill modifierSkill:
                isHit = true;
                //skill.UseSkill(Character, characterReceptor, out damage);
                break;
            case ReviveSkill reviveSkill:
                isHit = true;
                break;
            default:
                break;
        }

        if(isHit)
        {
            skill.UseSkill(Character, characterReceptor, out damage); 
        }

        OnAttackState(new AttackStateEventArgs{
            current = Character,
            receptor = characterReceptor,
            isHit = isHit,
            damage = damage,
            attack = skill,
            baseAction = this
        });

        characterReceptor.DataContainer.Hp -= damage; 
    }

    private void ExecuteSkill(Skill skill)
    {
        if(skill.AllReceptors)
        {
            foreach(Character characterReceptor in _characterReceptorList)
            {
                Skill(characterReceptor, skill); 
            }
        }
        else
        {
            Skill(_characterReceptorList[0], skill); 
        }

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

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        if(Character.DataContainer.Sp < _currentSkill.Cost)
        {
            OnCannotTakeAction();
            return;
        }

        _characterReceptorList.Clear();
        _characterReceptorList = characterReceptorList;
        OnActionComplete = onActionComplete;
        ExecuteSkill(_currentSkill);

        OnActionTaken();
    }

    public static bool CanUseSkill(Skill skill, Character character)
    {
        return character.DataContainer.Sp >= skill.Cost;
    }

    public override string GetActionName()
    {
        return "Skill";
    }

    private void SkillUI_OnConfirmSkill(object sender, SkillUI.OnConfirmSkillEventArgs e)
    {
        _currentSkill = e.skill;
    }

}
