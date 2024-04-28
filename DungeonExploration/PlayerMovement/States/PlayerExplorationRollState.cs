using Godot;
using System;

public partial class PlayerExplorationRollState : PlayerExplorationBaseState
{
    public override void OnEnter(PlayerExplorationMovement context)
    {
        Context = context;
        Context.CanMove = false;
        Context.AnimationPlayerHead.Play("roll");
        Context.CurrentSpeed = 2.5f;
    }

    public override void OnUpdate(float delta)
    {

    }

    private void OnAnimationPlayerHeadAnimationFinished(string AnimName)
    {
        if(AnimName == "roll")
        {
            Context.SwitchState(Context.idleState);
        }
    }

    public override string ToString()
    {
        return "Roll";
    }
}
