using Godot;
using System;
using System.Collections.Generic;

public partial class CharacterDataResource : Resource
{
    private int _hp;
    private int _hpMax;
    private int _sp;
    private int _spMax;
    private int _lv;
    private int _xp;
    private int _xpForNextLevel;
    private bool _isEnemy;
    private int _st;
    private int _ma;
    private int _ag; 
    private int _co;
    private int _lu;
    private List<BaseAction> _actionList = new List<BaseAction>();
    private List<SkillResource> _skillList = new List<SkillResource>();
    private BaseAction _selectedAction;
    private SkillResource _selectedSkill;
    private bool _isDefending;
    private int _weaponDamage; // Temp!!!
    private int _armorDefense; // Temp!!!


	[Export]
    public int Hp {get {return _hp;} set {_hp = value;}} 
	[Export]
    public int HpMax {get {return _hpMax;} set {_hpMax = value;}} 
	[Export]
    public int Sp {get {return _sp;} set {_sp = value;}} 
	[Export]
    public int SpMax {get {return _spMax;} set {_spMax = value;}} 
	[Export]
    public int Lv {get {return _lv;} set {_lv = value;}} 
	[Export]
    public int Xp {get {return _xp;} set {_xp = value;}} 
	[Export]
    public int XpForNextLevel {get {return _xpForNextLevel;} set {_xpForNextLevel = value;}} 
	[Export]
    public bool IsEnemy {get {return _isEnemy;} set {_isEnemy = value;}} 
	[Export]
    public int St {get {return _st;} set {_st = value;}} 
	[Export]
    public int Ma {get {return _ma;} set {_ma = value;}}
	[Export]
    public int Ag {get {return _ag;} set {_ag = value;}} 
	[Export]
    public int Co {get {return _co;} set {_co = value;}}
	[Export]
    public int Lu {get {return _lu;} set {_lu = value;}}
    [Export]
    public int WeaponDamage {get {return _weaponDamage;} set {_weaponDamage = value;}}
    [Export]
    public int ArmorDefense {get {return _armorDefense;} set {_armorDefense = value;}}
    // [Export] It's seems to be some kind of error when trying to export a list. Let's us wait.
    public List<BaseAction> ActionList {get {return _actionList; } set {_actionList = value; }}
    public List<SkillResource> SkillList {get {return _skillList; } set {_skillList = value; }}
    [Export]
    public BaseAction SelectedAction {get {return _selectedAction;} set {_selectedAction = value;}}
    [Export]
    public SkillResource SelectedSkill {get {return _selectedSkill;} set {_selectedSkill = value;}}
    [Export]
    public bool IsDefending {get {return _isDefending;} set {_isDefending = value;}}

    public void IncreaseHpMax()
    {
        _hpMax = _hpMax + 30 + _co / 2;
    }

    public void IncreaseSpMax()
    {
        _spMax += 15 + _ma / 4;
    }

    public int IncreaseStat(int stat)
    {
        return stat += GD.RandRange(1, 3);
    }
}
