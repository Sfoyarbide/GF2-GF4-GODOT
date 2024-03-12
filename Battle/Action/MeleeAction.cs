using Godot;
using System;

public partial class MeleeAction : BaseAction
{
    private Character _characterReceptor;
    private Transform3D _previousTransform;
    public static event EventHandler<OnMeleeEventArgs> OnMeleeStarted;
    public static event EventHandler<OnMeleeEventArgs> OnMelee;
    public static event EventHandler<OnMeleeEventArgs> OnMeleeEnd;
    public class OnMeleeEventArgs : EventArgs
    {
        public Character character;
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

	public void Attack(Character characterReceptor) // Checks if you hit, and execute the attack.
    {
        bool isHit = CombatCalculations.IsHitCalculation(Character, characterReceptor);
        int damage = 0;
        if(isHit)
        {
            damage = CombatCalculations.DamageCalculation(Character, characterReceptor, Character.DataContainer.MeleeAttack);
            
            if(CombatCalculations.IsCriticHitCalculation(Character, characterReceptor))
            {
                damage *= 2;
                KnockDown knockDown = new KnockDown();
                knockDown.InflictCharacter(characterReceptor);
            }

            // Checks if the receptor was defending, and cancels it.
            DefendAction.TryCancelDefend(characterReceptor);
            characterReceptor.DataContainer.Hp -= damage; 
        }
        
        OnAttackState(new AttackStateEventArgs{
            receptor = _characterReceptor,
            isHit = isHit,
            damage = damage,
            attack = Character.DataContainer.MeleeAttack
        });
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
            OnMelee?.Invoke(this, new OnMeleeEventArgs{
                character = Character
            });

            // Does all the Attack Action logic. 
            Attack(_characterReceptor);

            InAction = false;
            EndingAction();
        }
    }

    public async override void EndingAction()
    {
        Timer.Start();
        await ToSignal(Timer, Timer.SignalName.Timeout);

        OnMeleeEnd?.Invoke(this, new OnMeleeEventArgs{
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

        OnMeleeStarted?.Invoke(this, new OnMeleeEventArgs{
            character = Character
        });
    }

    public override string GetActionName()
    {
        return "Attack";
    }
}