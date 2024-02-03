using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class SkillUI : Node
{
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
            OnConfirmSkill?.Invoke(this, new OnConfirmSkillEventArgs{
                skill = _skill
            });
        };
    }

    public void Setup(Skill skill)
    {
        _skill = skill;
        _skillIconUI.Texture = skill.SkillIcon;
        _skillNameUI.Text = skill.SkillName;
        _skillCostUI.Text = skill.SpCost.ToString();
    }
}