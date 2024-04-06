using Godot;
using System;

public partial class CharacterEnemyData : CharacterData
{
    private Character _lockTarget;

    public Character LockTarget { get {return _lockTarget; } set {_lockTarget = value;}}
}