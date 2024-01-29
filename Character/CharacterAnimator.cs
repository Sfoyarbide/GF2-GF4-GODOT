using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class CharacterAnimator : Node3D
{
    private Character _character;
    private AnimationTree _animationTree;
    [Export]
    public bool _isMoving;
    [Export]
    public bool _attackMelee;
    [Export]
    public bool _attackSkill;
    [Export]
    public bool _healSkill;
    [Export]
    public bool _isDefending;
    [Export]
    public bool _isHit;


    public override void _Ready()
    {
        _animationTree = GetNode<AnimationTree>("AnimationTree");
        AttackAction.OnAttackStarted += AttackAction_OnAttackStarted;
        AttackAction.OnAttack += AttackAction_OnAttack;
        AttackAction.OnAttackEnd += AttackAction_OnAttackEnd;
        SkillAction.OnSkillCast += SkillAction_OnSkillCast;
        SkillAction.OnSkillEnd += SkillAction_OnSkillEnd;
        DefendAction.OnDefend += DefendAction_OnDefend;
        DefendAction.OnCancelDefend += DefendAction_OnCancelDefend;
        ItemAction.OnUsedItemStarted += ItemAction_OnUsedItemStarted;
        CharacterDataResource.OnHpChanged += CharacterDataResource_OnHpChanged;
    }

    public void Setup(Character character)
    {
        _character = character;
    }

    public void SetBool(ref bool animationCondition, bool value)
    {
        animationCondition = value;
    }

    public void SetTrigger(ref bool animationCondition)
    {
        animationCondition = true;
    }

    private void AttackAction_OnAttackStarted(object sender, AttackAction.OnAttackEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _isMoving, true);
        }
    }

    private void AttackAction_OnAttack(object sender, AttackAction.OnAttackEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _attackMelee, true);
        }
    }

    private void AttackAction_OnAttackEnd(object sender, AttackAction.OnAttackEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _attackMelee, false);
            SetBool(ref _isMoving, false);
        }
    }

    private void ItemAction_OnUsedItemStarted(object sender, ItemAction.OnUsedItemEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _healSkill, true);
        }
    }

    private void SkillAction_OnSkillCast(object sender, SkillAction.OnSkillEventArgs e)
    {
        if(e.character == _character)
        {
            switch(e.skill.SkillType)
            {
                case SkillType.Heal:
                    SetBool(ref _healSkill, true);
                    break;
                default:
                    SetBool(ref _attackSkill, true);
                    break;
            }
        }
    }

    private void SkillAction_OnSkillEnd(object sender, SkillAction.OnSkillEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _attackSkill, false);
            SetBool(ref _healSkill, false);
        }
    }

    private void DefendAction_OnDefend(object sender, DefendAction.OnDefendEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _isDefending, true);
        }
    }

    private void DefendAction_OnCancelDefend(object sender, DefendAction.OnDefendEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _isDefending, false);
        }
    }

    private void CharacterDataResource_OnHpChanged(object sender, CharacterDataResource.OnHpChangedEventArgs e)
    {
        if(e.character == _character)
        {
            SetBool(ref _isHit, true);
        }
    }

    private void OnAnimationTreeAnimationFinished(StringName stringName)
    {
        switch(stringName)
        {
            case "Hit":
                SetBool(ref _isHit, false);
                break;
            case "Heal_Spell":
                SetBool(ref _healSkill, false);
                break;
        }
    }
}