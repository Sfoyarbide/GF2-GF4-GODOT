using Godot;
using System;
using System.Collections.Generic;

public enum ElementStatus
{
    Normal,
    Weakness,
    Resistance,
    Null,
    Absorb, 
    Reflect
}

public partial class Character : Node3D
{
    private Node _actionContainer; 
    private Node3D _markerContainer;
    private CharacterData _dataContainer;
    private CharacterAnimator _characterAnimation;
    [Export	]
    private float _combatPositionModifier;
    private bool _inCombat;

    public CharacterData DataContainer {get {return _dataContainer;} set {_dataContainer = value;}}
    public float CombatPositionModifier {get {return _combatPositionModifier;}}
    public bool InCombat {get {return _inCombat; } set {_inCombat = value;}}

    public override void _Ready()
    {
        _dataContainer = GetNode<CharacterData>("Data");
        _actionContainer = GetNode("ActionContainer");
        _markerContainer = GetNode<Node3D>("MarkerContainer");
        _characterAnimation = GetNode<CharacterAnimator>("Pivot/Model");
        
        _characterAnimation.Setup(this);

        CharacterData.OnDie += CharacterData_OnDie;
        DataContainer.Character = this;

        UpdateActionList();

        DataContainer.Hp = DataContainer.HpMax;
        DataContainer.SkillNameList.Add("Fire");
        DataContainer.SkillNameList.Add("Ice");
        DataContainer.SkillNameList.Add("AttackUP");
        DataContainer.SkillNameList.Add("AttackDOWN");
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

    private void CharacterData_OnDie(object sender, CharacterData.CharacterDataEventArgs e)
    {
        if(e.character == this)
        {
            _inCombat = false;
        }
    }
}
