using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node3D
{
    [Export]
    private CharacterDataResource _dataContainer;
    [Export]
    private SkillResource skillResource;
    private Node _actionContainer;

    public CharacterDataResource DataContainer {get {return _dataContainer;} set {_dataContainer = value;}}

    public override void _Ready()
    {
        _actionContainer = GetNode("ActionContainer");

        UpdateActionList();

        // TEMP
        DataContainer.SelectedAction = DataContainer.ActionList[2];
        DataContainer.SelectedSkill = skillResource;
        _dataContainer.SelectedAction = _dataContainer.ActionList[0];
    }

    public void UpdateActionList()
    {
        _dataContainer.ActionList.Clear();
        foreach(BaseAction actionChild in _actionContainer.GetChildren())
        {
            actionChild.Character = this;
            _dataContainer.ActionList.Add(actionChild);
        }
    }

    public override string ToString()
    {
        return Name;
    }
}
