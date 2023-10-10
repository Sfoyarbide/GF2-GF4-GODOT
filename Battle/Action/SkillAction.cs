using Godot;
using System;
using System.Collections.Generic;

public partial class SkillAction : BaseAction
{
    private List<Character> characterReceptorList = new List<Character>();

    public override void _Ready()
    {
        base._Ready();  
    }

    private bool IsSkill(Character characterReceptor, SkillResource skill, out int outDamage)
    {  
        if(Character.DataContainer.Sp < skill.SpCost)
        {
            outDamage = 0;
            return false;
        }

        int maCharacter = Character.DataContainer.Ma;
        int agCharacter = Character.DataContainer.Ag;
        int characterChance = maCharacter + agCharacter;

        int agCharacterReceptor = characterReceptor.DataContainer.Ag;

        int dice = GD.RandRange(0,10);
        
        int newSp = Character.DataContainer.Sp - skill.SpCost;
        Character.DataContainer.Sp = newSp;

        if(CombatCalculations.CheckIsHit(characterChance, agCharacterReceptor, dice))
        {
            Skill(characterReceptor, skill, out outDamage);
            GD.Print("Action: SkillAction, Skill: " + skill + ", Damage: " + outDamage + ", Receptor Hp: " + characterReceptor.DataContainer.Hp + ", Receptor ID: " + characterReceptor);
            return true;
        }
        else
        {
            outDamage = 0;
            GD.Print("Action: SkillAction, Skill: " + skill + ", Damage: " + "Not hit" + ", Receptor Hp: " + characterReceptor.DataContainer.Hp  + ", Receptor ID: " + characterReceptor);
            return false;
        }
    }

    private void Skill(Character characterReceptor, SkillResource skill, out int outDamage)
    {
        int hp = characterReceptor.DataContainer.Hp; // Getting receptor's hp. 
        int armorDefense = characterReceptor.DataContainer.ArmorDefense;
        int damage;

        damage = CombatCalculations.CalculateDamage(skill.Damage, armorDefense);
        outDamage = damage;

        if(damage <= 0)
        {
            // Logic code to do.
            return;
        }
        
        int newHp = hp - damage;
        characterReceptor.DataContainer.Hp = newHp;
    }

    private void HealSkill(Character characterReceptor, SkillResource skill)
    {
        int hp = characterReceptor.DataContainer.Hp; // Getting receptor's hp. 
        int healAmount = skill.Damage; // baseDamage is refering to the healAmount in this case.
        int newHp = hp + healAmount;
        characterReceptor.DataContainer.Hp = newHp;
    }

    private void ExecuteSkill(SkillResource skill)
    {
        if(skill.SkillType != SkillType.Heal) 
        {
            InvokeSkillToCharacterReceptorList(characterReceptorList, skill);
        }
        else // If the skill type is healing, you cannot fail the cast, therefore you use HealSkill function.
        {
            InvokeHealSkillToCharacterReceptorList(characterReceptorList, skill);
        }

        OnActionComplete();
    }

    private void InvokeSkillToCharacterReceptorList(List<Character> characterReceptorList, SkillResource skill)
    {
        if(skill.IsAllReceiveDamage)
        {
            foreach(Character characterReceptor in characterReceptorList)
            {
                bool isSkill = IsSkill(characterReceptor, skill, out int outDamage); 
                /*
                OnAttackStatus?.Invoke(this, new OnAttackStateEventArgs{
                    characterReceptor = characterReceptor,
                    attackStatus = isSkill,
                    damage = outDamage
                });
                */
            }
        }
        else
        {
            bool isSkill = IsSkill(characterReceptorList[0], skill, out int outDamage); 
            /*
            OnAttackStatus?.Invoke(this, new OnAttackStateEventArgs{
                characterReceptor = characterReceptorList[0],
                attackStatus = isSkill,
                damage = outDamage
            });
            */
        }
    }

    private void InvokeHealSkillToCharacterReceptorList(List<Character> characterReceptorList, SkillResource skill)
    {
        if(skill.IsAllReceiveDamage)
        {
            foreach(Character characterReceptor in characterReceptorList)
            {
                HealSkill(characterReceptor, skill);
                /*
                OnAttackStatus?.Invoke(this, new OnAttackStateEventArgs{
                    characterReceptor = characterReceptor,
                    attackStatus = true,
                    damage = skill.baseDamage
                });
                */
            }
        }
        else
        {
            HealSkill(characterReceptorList[0], skill);
            /*
            OnAttackStatus?.Invoke(this, new OnAttackStateEventArgs{
                    characterReceptor = characterReceptorList[0],
                    attackStatus = true,
                    damage = skill.baseDamage
            });
            */
        }
    }

    private SkillResource GetCurrentSkill()
    {
        return Character.DataContainer.SelectedSkill;
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        SkillResource skill = GetCurrentSkill();
        characterReceptorList.Add(characterReceptor);
        OnActionComplete = onActionComplete;
        ExecuteSkill(skill);
    }

    public override void TakeAction(List<Character> characterReceptorList, Action onActionComplete)
    {
        SkillResource skill = GetCurrentSkill();
        this.characterReceptorList = characterReceptorList;
        OnActionComplete = onActionComplete;
        ExecuteSkill(skill);
    }

    public override string GetActionName()
    {
        return "Skill";
    }
}
