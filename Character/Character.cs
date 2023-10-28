using Godot;
using System;
using System.Collections.Generic;

public partial class Character : Node3D
{
    private Node _actionContainer; 
    private Node3D _markerContainer;
    [Export]
    private CharacterDataResource _dataContainer;
    private CharacterAnimator _characterAnimation;
    public CharacterDataResource DataContainer {get {return _dataContainer;} set {_dataContainer = value;}}

    // TEMP
    [Export]
    private SkillResource _skillResource;
    [Export]
    private int INDEX;


    public override void _Ready()
    {
        _actionContainer = GetNode("ActionContainer");
        _markerContainer = GetNode<Node3D>("MarkerContainer");
        _characterAnimation = GetNode<CharacterAnimator>("Pivot/Model");
        
        _characterAnimation.Setup(this);

        DataContainer.Character = this;

        UpdateActionList();



        // TEMP
        DataContainer.SelectedSkill = _skillResource;
        _dataContainer.SelectedAction = _dataContainer.ActionList[INDEX];
    }

    public void UpdateActionList()
    {
        if(_dataContainer.ActionList.Count > 0)
        {
            _dataContainer.ActionList.Clear();
        }
        foreach(BaseAction actionChild in _actionContainer.GetChildren())
        {
            actionChild.Character = this;
            _dataContainer.ActionList.Add(actionChild);
        }
    }

    public Transform3D GetMarkerChildTransform(int index)
    {
        return _markerContainer.GetChild<Marker3D>(index).GlobalTransform;
    }  

    public override string ToString()
    {
        return Name;
    }
}
