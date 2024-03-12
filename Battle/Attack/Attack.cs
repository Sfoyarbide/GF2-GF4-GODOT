using Godot;
using System;

public partial class Attack : Node
{
    [Export]
    private int _damage;
    [Export]
    private int _cost;
    [Export]
    private AttackTypes _attackType;

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