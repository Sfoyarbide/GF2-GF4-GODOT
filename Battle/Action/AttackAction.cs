using Godot;
using System;

public partial class AttackAction : BaseAction
{
	private int _potencialDamage = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public bool IsAttack(Character characterReceptor) // Checks if you hit, and execute the attack.
    {
        int agCharacter = Character.DataContainer.Ag;
        int agCharacterReceptor = characterReceptor.DataContainer.Ag;
        int dice = GD.RandRange(0, 10);
        if(CombatCalculations.CheckIsHit(agCharacter, agCharacterReceptor, dice))
        {
            Attack(characterReceptor);
            GD.Print("Action: AttackAction, Damage: " + _potencialDamage + ", Receptor Hp: " + characterReceptor.DataContainer.Hp + ", Receptor ID: " + characterReceptor);
            return true;
        }
        else
        {
            _potencialDamage = 0;
             GD.Print("Action: AttackAction, Damage: Not hit" + ", Receptor Hp: " + characterReceptor.DataContainer.Hp + ", Receptor ID: " + characterReceptor);
            return false;
        }
    }

    public void Attack(Character characterReceptor)
    {
        int hp = characterReceptor.DataContainer.Hp; // Getting receptor's hp. 
        int weaponDamage = characterReceptor.DataContainer.WeaponDamage; // Getting character's weapon damage. 
        int armorDefense = characterReceptor.DataContainer.ArmorDefense; // Getting receptor's armor defense.
        int bonusDamage = Character.DataContainer.St; 
		int damage = 0; 

        damage = CombatCalculations.CalculateDamage(weaponDamage, armorDefense);

        if(IsCriticHit(characterReceptor)) // Check is the hit is critical
        {
            GD.Print("Critical");
            damage *= 2; // Critical hit is the damage multiplied by two. 
        }

        damage += bonusDamage; // Adding the damage bonus. 
        _potencialDamage = damage;
        
        DefendAction.CancelDefend(characterReceptor); // Checks if the receptor is defending, and executes the logic of canceling a defend.

        int newHp = hp - damage; // Final subtraction. 
        characterReceptor.DataContainer.Hp = newHp; // Setting new hp value for the character receptor.
    }

    public bool IsCriticHit(Character characterReceptor)
    {
        int luCharacter = Character.DataContainer.Lu;
        int agCharacter = characterReceptor.DataContainer.Ag;
        int dice = GD.RandRange(0,10);
        if((luCharacter * dice) - agCharacter > agCharacter * 2) // Formula to check if the hit is critic.
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        IsAttack(characterReceptor);
		OnActionComplete = onActionComplete;
		OnActionComplete();
    }

    public override string GetActionName()
    {
        return "Attack";
    }

}
