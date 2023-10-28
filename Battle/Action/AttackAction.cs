using Godot;
using System;

public partial class AttackAction : BaseAction
{
	private int _potencialDamage = 0;
    private Character _characterReceptor;
    private Transform3D _previousTransform;
    public static event EventHandler<OnAttackEventArgs> OnAttackStarted;
    public static event EventHandler<OnAttackEventArgs> OnAttack;
    public static event EventHandler<OnAttackEventArgs> OnAttackEnd;
    public static event EventHandler AttackState;
    public class OnAttackEventArgs : EventArgs
    {
        public Character character;
    }
    public class AttackStateEventArgs : EventArgs
    {
        public Character characterReceptor;
        public bool attackStatus;
        public int damage;
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        ExecuteAttack((float)delta);
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

    private void ExecuteAttack(float delta) // Does all the visual and logic aspects of Attack Action.
    {
        if(!InAction)
        {
            return;
        }
    
        float speedMovement = 5;
        float speedRotation = 5;

        Transform3D damagePointTransform = _characterReceptor.GetMarkerChildTransform(2);
        // Finding the direction.
        Vector3 direction = (damagePointTransform.Origin - Character.GlobalTransform.Origin).Normalized();
        // Translate the Character.
        Character.GlobalTranslate(direction * delta * speedMovement);

        // Finding the angle.
        float angle = Mathf.Atan2(direction.X, direction.Z);
        // Obtaining the lerp angle.
        float lerpAngle = Mathf.LerpAngle(Character.GlobalRotation.Y, angle, delta * speedRotation);
        // Creating the vector Rotation.
        Vector3 rotation = Vector3.Zero;
        // Applying in the Y axis, the lerpAngle.  
        rotation.Y = lerpAngle;
        // Applying to the Rotation to GlobalRotation 
        Character.GlobalRotation = rotation;

        if((damagePointTransform.Origin - Character.GlobalTransform.Origin).Length() < 0.5f)
        {
            OnAttack?.Invoke(this, new OnAttackEventArgs{
                character = Character
            });

            // Does all the Attack Action logic. 
            bool isAttack = IsAttack(_characterReceptor); 

            // Invokes the event for the visual aspect of the attack state.
            AttackState?.Invoke(this, new AttackStateEventArgs{
                characterReceptor = _characterReceptor,
                attackStatus = isAttack,
                damage = _potencialDamage
            }); // Attack status.
            
            //Invoke("AfterAttackActionIsComplete", 1f);

            InAction = false;
            EndingAction();
        }
    }

    public async override void EndingAction()
    {
        Timer.Start();
        await ToSignal(Timer, Timer.SignalName.Timeout);

        OnAttackEnd?.Invoke(this, new OnAttackEventArgs{
                character = Character
        });

        Character.GlobalTransform = _previousTransform;
        
        OnActionComplete();
    }

    public override void TakeAction(Character characterReceptor, Action onActionComplete)
    {
        _characterReceptor = characterReceptor;
		OnActionComplete = onActionComplete;

        InAction = true;
        _previousTransform = Character.GlobalTransform;

        OnAttackStarted?.Invoke(this, new OnAttackEventArgs{
            character = Character
        });
		//OnActionComplete();
    }

    public override string GetActionName()
    {
        return "Attack";
    }
}
