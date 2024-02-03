using Godot;
using System;

public partial class SkillContainerUI : Control
{
    [Export]
    private PackedScene _skillUIScene;
    private VBoxContainer _skillContainerUI;

    public override void _Ready()
    {
        BattleDatabase battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        battleDatabase.BattleManager.OnSkillSelectionStarted += BattleManager_OnSkillSelectionStarted;
        SkillUI.OnConfirmSkill += SkillUI_OnConfirmSkill;

        if(_skillUIScene == null)
        {
            GD.PrintErr("Skill UI Scene has not been assigned. Plese add it in Skill Container UI.");
        }

        _skillContainerUI = GetNode<VBoxContainer>("Panel/ScrollContainer/VBoxContainer");
    }

    public void ShowSkillContainerUI(Character character)
    {
        ClearInventoryUI();
        // Only do this if has been changed. Add this.
        foreach(Skill skill in character.DataContainer.SkillList)
        {
            SkillUI skillUI = (SkillUI)_skillUIScene.Instantiate();
            _skillContainerUI.AddChild(skillUI);
            skillUI.Setup(skill);
        }
    }

    public void ClearInventoryUI()
    {
        for(int x = 0; x < _skillContainerUI.GetChildCount(); x++)
        {
            _skillContainerUI.GetChild(x).QueueFree();
        }
    }

    private void BattleManager_OnSkillSelectionStarted(object sender, BattleManager.OnSkillSelectionStartedEventArgs e)
    {
        Show();
        ShowSkillContainerUI(e.character);
    }

    private void SkillUI_OnConfirmSkill(object sender, SkillUI.OnConfirmSkillEventArgs e)
    {
        Hide();
    }
}