using Godot;
using System;

public partial class PlayerExplorationFallState : PlayerExplorationBaseState
{
    private float _inicialPosY;
    private float _finalPosY;
    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        _inicialPosY = Context.GlobalPosition.Y;
    }

    public override void OnUpdate(float delta)
    {
        if(Context.IsOnFloor())
        {   
            _finalPosY = Context.GlobalPosition.Y;

            if(_inicialPosY - _finalPosY >= 3f)
            {
                Context.SwitchState(Context.rollState);
                return;
            }

            Context.SwitchState(Context.idleState);
        }
    }

    public override string ToString()
    {
        return "Fall";
    }
}
