using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class SkillUI : Node
{
    private Character _character;
    private Skill _skill;
    private Label _skillNameUI;
    private Label _skillCostUI;
    private TextureRect _skillIconUI;
    private Button _confirmSkillUI;

    public static event EventHandler<OnConfirmSkillEventArgs> OnConfirmSkill;
    public class OnConfirmSkillEventArgs : EventArgs
    {
        public Skill skill;
    }

    public override void _Ready()
    {
        _skillIconUI = GetNode<TextureRect>("SkillIconUI");
        _skillNameUI = GetNode<Label>("SkillNameUI");
        _skillCostUI = GetNode<Label>("SkillCostUI");
        _confirmSkillUI = GetNode<Button>("ConfirmSkillUI");

        _confirmSkillUI.Pressed += () => {

            if(!SkillAction.CanUseSkill(_skill, _character))
            {
                // Make some sounds
                return;
            }

            OnConfirmSkill?.Invoke(this, new OnConfirmSkillEventArgs{
                skill = _skill
            });
        };
    }

    public void Setup(Character character, Skill skill)
    {
        _character = character;
        _skill = skill;
        _skillIconUI.Texture = skill.SkillIcon;
        _skillNameUI.Text = skill.SkillName;
        _skillCostUI.Text = skill.Cost.ToString();
    }
}