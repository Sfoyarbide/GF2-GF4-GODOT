using Godot;
using System;
using System.Collections.Generic;

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
        InflictState inflictState = null; // We define it in order to save if we inflict a state.

        if(isHit)
        {
            Attack meleeAttack = Character.DataContainer.MeleeAttack; 
            damage = CombatCalculations.DamageCalculation(Character, characterReceptor, meleeAttack);

            if(CombatCalculations.IsCriticHitCalculation(Character, characterReceptor) && !characterReceptor.DataContainer.IsDefending)
            {
                damage *= 2;
                //inflictState = new KnockDown();
                //inflictState.InflictCharacter(characterReceptor);
            }

            // Checks if the receptor was defending, and cancels it.
            DefendAction.TryCancelDefend(characterReceptor);
        }
        
        OnAttackState(new AttackStateEventArgs{
            current = Character,
            receptor = _characterReceptor,
            isHit = isHit,
            damage = damage,
            attack = Character.DataContainer.MeleeAttack,
            inflictState = inflictState,
            baseAction = this
        });

        characterReceptor.DataContainer.Hp -= damage; 
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

            EndingAction();
            InAction = false;
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

    public override void TakeAction(List<Character> characterReceptor, Action onActionComplete)
    {
        _characterReceptor = characterReceptor[0];
		OnActionComplete = onActionComplete;

        InAction = true;
        _previousTransform = Character.GlobalTransform;

        OnMeleeStarted?.Invoke(this, new OnMeleeEventArgs{
            character = Character
        });

        OnActionTaken();
    }

    public override string GetActionName()
    {
        return "Attack";
    }
}
