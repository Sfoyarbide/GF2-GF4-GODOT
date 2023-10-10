using Godot;
using System;

public partial class DefendAction : BaseAction
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	private void Defend()
    {
        int armorDefense = Character.DataContainer.ArmorDefense;
        int newArmorDefense = armorDefense * 2;
        Character.DataContainer.ArmorDefense = newArmorDefense; 
        Character.DataContainer.IsDefending = true; // The event IsDefendingChanged is invoke in this function.
        GD.Print("Action: " + GetActionName() + ", Defend Status: true, ArmorDefense: " + armorDefense);
    }

    public static void CancelDefend(Character characterReceptor)
    {
        if(characterReceptor.DataContainer.IsDefending)
        {
            int armorDefense = characterReceptor.DataContainer.ArmorDefense;
            int newArmorDefense = armorDefense / 2;
            characterReceptor.DataContainer.IsDefending = false;
            characterReceptor.DataContainer.ArmorDefense = newArmorDefense;
            GD.Print("Action: DefendAction" + ", Defend Status: false, ArmorDefense: " + armorDefense);
        }
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        OnActionComplete = onActionComplete;
		// Because the receptor is same as the character emisor, we don't use as a parameter.
		Defend();
    }

    public override string GetActionName()
    {
        return "Defend";
    }

}
