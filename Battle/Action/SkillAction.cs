using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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

    private void Skill(Character characterReceptor, Skill skill)
    {  
        if(Character.DataContainer.Sp < skill.Cost)
        {
            return;
        }

        Character.DataContainer.Sp -= skill.Cost;
        
        int damage = 0;
        switch(skill)
        {
            case HealSkill healSkill:
                skill.UseSkill(Character, characterReceptor, out damage);
                return;
            case ModifierSkill modifierSkill:
                skill.UseSkill(Character, characterReceptor, out damage);
                return;
            default:
                break;
        }

        bool isHit = CombatCalculations.IsHitCalculation(Character, characterReceptor);
        if(isHit)
        {
            skill.UseSkill(Character, characterReceptor, out damage); 
            if(characterReceptor.DataContainer.IsElementStatusToAttackType(skill.AttackType, ElementStatus.Weakness) && !characterReceptor.DataContainer.IsDefending)
            {
                characterReceptor.DataContainer.AlreadyHitWeakness = true;
            }
        }

        OnAttackState(new AttackStateEventArgs{
            current = Character,
            receptor = characterReceptor,
            isHit = isHit,
            damage = damage,
            attack = skill,
            baseAction = this
        });
    }

    private void InvokeSkillToCharacterReceptorList(List<Character> characterReceptorList, Skill skill)
    {
        if(skill.IsAllReceiveDamage)
        {
            foreach(Character characterReceptor in characterReceptorList)
            {
                Skill(characterReceptor, skill); 
            }
        }
        else
        {
            Skill(characterReceptorList[0], skill); 
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
        _characterReceptorList.Clear();
        _characterReceptorList.Add(characterReceptor);
        OnActionComplete = onActionComplete;
        ExecuteSkill(_currentSkill);
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        _characterReceptorList.Clear();
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
