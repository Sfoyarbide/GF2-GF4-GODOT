using Godot;
using System;
using System.Collections.Generic;


public partial class Attack : Node
{
    [Export]
    private string _attackName;
    [Export]
    private int _damage;
    [Export]
    private int _cost;
    [Export]
    private bool _allReceptors;
    private List<ReceptorCriteria> receptorCriteriaList = new List<ReceptorCriteria>();
    [Export]
    private AttackTypes _attackType;

    public string AttackName
    {
        get {return _attackName;}
        set {_attackName = value;}
    }

    public int Damage 
    {
        get {return _damage;}
        protected set {_damage = value;}
    }

    public int Cost
    {
        get {return _cost;}
        protected set {_cost = value;}
    }

    public bool AllReceptors
    {
        get { return _allReceptors; }
        set { _allReceptors = value; }
    }

    public List<ReceptorCriteria> ReceptorCriteriaList 
    { 
        get { return receptorCriteriaList; }
        set { receptorCriteriaList = value; } 
    }

    public AttackTypes AttackType
    {
        get {return _attackType;}
        protected set {_attackType = value;}
    }
}

public enum AttackTypes
{
    Strike,
    Slash,
    Pierce,
    Fire,
    Ice,
    Wind,
    Light,
    Heal,
    Inflict
}