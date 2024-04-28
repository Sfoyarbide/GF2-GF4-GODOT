using Godot;
using System;

public partial class PlayerExplorationMovement : CharacterBody3D
{
    // States
    private PlayerExplorationBaseState _currentState;
    public PlayerExplorationBaseState idleState;
    public PlayerExplorationBaseState walkingState;
    public PlayerExplorationBaseState sprintingState;
    public PlayerExplorationJumpState jumpState;
    public PlayerExplorationBaseState crouchingState;
    public PlayerExplorationBaseState slidingState;
    public PlayerExplorationBaseState wallState;
    public PlayerExplorationBaseState rollState;
    public PlayerExplorationBaseState fallState;

    // bool states.
    private bool _canMove;

    private bool _isFreelooking;
    private Node _stateContainer;
    [Export]
    private Label _stateDebuger;

    // Speed and Forces vars.
    [Export]
    private float _currentSpeed = 5f;
    [Export]
    private float _jumpForce = 3.5f;
    private float _gravity = 9.8f;

    // Position vars.
    private Vector3 _direction;
    public float yHeadStandPosition = 0.8f;
    public float yPositionCrouchHead = 0.3f;
    private float lastInicialPosY;
    private float lastFinalPosY;

    // Child Nodes vars.
    private Node3D _neck;
    private Node3D _head;
    private Camera3D _camera;

    private CollisionShape3D _standCollisionShape;
    private CollisionShape3D _crouchCollisionShape;

    private RayCast3D _floorChecker;
    private RayCast3D _wallChecker;
    private RayCast3D _crouchChecker;

    // Sensitivity vars.
    private float _freeLookTiltAmount = 40f;
    private float _lerpMovementSensitivity = 20f;
    private float _mouseSensitivity = 0.08f;

    // Animation Vars
    private AnimationPlayer _animationPlayerHead;

    // Getters and Setters
    public Vector3 Direction {get {return _direction;} set {_direction = value;}}
    public bool IsFreelooking {get {return _isFreelooking; } set {_isFreelooking = value;}}
    public float JumpForce {get {return _jumpForce; } set {_jumpForce = value;}}
    public float CurrentSpeed {get {return _currentSpeed; } set {_currentSpeed = value;}}
    public bool CanMove {get {return _canMove;} set {_canMove = value;}}
    public Node3D Head {get {return _head;}}
    public Node3D Neck {get {return _neck;}}
    public Label StateDebuger {get {return _stateDebuger;}}
    public Camera3D Camera3D {get {return _camera; } set {_camera = value;}}
    public CollisionShape3D StandCollisionShape {get {return _standCollisionShape;}}
    public CollisionShape3D CrouchCollisionShape {get {return _crouchCollisionShape;}}
    public RayCast3D CrouchChecker {get {return _crouchChecker;}}
    public AnimationPlayer AnimationPlayerHead {get {return _animationPlayerHead;} set {_animationPlayerHead = value;}}

    public override void _Ready()
    {
        // Finding Nodes.
        // Body.
        _neck = GetNode<Node3D>("Neck");
        _head = _neck.GetNode<Node3D>("Head");
        _camera = _head.GetNode<Camera3D>("Camera3D");

        // Collision Shape.
        _standCollisionShape = GetNode<CollisionShape3D>("StandCollisionShape");
        _crouchCollisionShape = GetNode<CollisionShape3D>("CrouchCollisionShape");
        
        // RayCast Checkers.
        _crouchChecker = _crouchCollisionShape.GetNode<RayCast3D>("CrouchChecker");
        _wallChecker = _standCollisionShape.GetNode<RayCast3D>("WallChecker");
        _floorChecker = _standCollisionShape.GetNode<RayCast3D>("FloorChecker");

        // States.
        _stateContainer = GetNode<Node>("StateContainer");
        idleState = _stateContainer.GetNode<PlayerExplorationIdleState>("Idle");
        walkingState = _stateContainer.GetNode<PlayerExplorationWalkingState>("Walking");
        jumpState = _stateContainer.GetNode<PlayerExplorationJumpState>("Jump");
        crouchingState = _stateContainer.GetNode<PlayerExplorationCrouchingState>("Crouching");
        sprintingState = _stateContainer.GetNode<PlayerExplorationSprintingState>("Sprinting");
        slidingState = _stateContainer.GetNode<PlayerExplorationSlidingState>("Sliding");
        wallState = _stateContainer.GetNode<PlayerExplorationWallState>("Wall");
        rollState = _stateContainer.GetNode<PlayerExplorationRollState>("Roll");
        fallState = _stateContainer.GetNode<PlayerExplorationFallState>("Fall");

        // Animation.
        _animationPlayerHead = Head.GetNode<AnimationPlayer>("AnimationPlayerHead");

        // Setting Default State.
        _currentState = idleState;
        _currentState.OnEnter(this);

        // Capturing Mouse.
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Input(InputEvent @event)
    {
        if(@event is InputEventMouseMotion mouseMotion)
        {
            if(!_isFreelooking)
            {
                RotateCamera(mouseMotion);
            }
            else
            {
                FreelookCamera(mouseMotion);
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // Handle Direction Calculation.
        if(_canMove)
        {
            Vector2 input = Input.GetVector("left","right","forward","down");
            Vector3 finalDirection = (Transform.Basis * new Vector3(input.X, 0, input.Y)).Normalized();
            LerpDirection(finalDirection, (float)delta);
        }

        if(!_isFreelooking && !Neck.Rotation.Equals(Vector3.Zero))
        {
            LerpNeck(0f, (float)delta);
        }

        // Generic All Valid Methods in all States.
        if(!IsOnFloor())
        {
            Fall((float)delta);
        }

        Move();
        
        // Update Current State

        _stateDebuger.Text = 
        "State: " + _currentState.ToString() + 
        "\nGlobalPosition: " + GlobalPosition +
        "\nCamGlobalRotation: " + Camera3D.Rotation;
        _currentState.OnUpdate((float)delta);

        MoveAndSlide();
    }

    public void SwitchState(PlayerExplorationBaseState newState)
    {

        _currentState = newState;
        _currentState.OnEnter(this);
    }

    public void Move()
    {
        if(!_direction.Equals(Vector3.Zero))
        {
            Velocity = new Vector3(_direction.X * _currentSpeed, Velocity.Y, _direction.Z * _currentSpeed);
        }
        else
        {
            float xFinalVelocity = Mathf.MoveToward(Velocity.X, 0, _currentSpeed);
            float zFinalVelocity = Mathf.MoveToward(Velocity.Z, 0, _currentSpeed);
            Velocity = new Vector3(xFinalVelocity, Velocity.Y, zFinalVelocity);
        }
    }

    public void Fall(float delta)
    {
        Velocity -= new Vector3(0, _gravity, 0) * delta;
    }

    public void Jump(float jumpForce)
    {
        Velocity = new Vector3(Velocity.X, jumpForce, Velocity.Z);
    }

    public void LerpHead(float yPositionWanted, float delta)
    {
        float yHeadPosition = _head.Position.Y;
        bool standPosition = true;

        // If our the Y Position Wanted is less than the current Y Head Position means that we are not going to stand. 
        if(yPositionWanted < yHeadPosition)
        {
            standPosition = false;
        }
        
        yHeadPosition = Mathf.Lerp(_head.Position.Y, yPositionWanted, delta * _lerpMovementSensitivity);

        if(yHeadPosition <= yPositionWanted && !standPosition)
        {
            yHeadPosition = yPositionWanted;
        }
        else if(yHeadPosition >= yPositionWanted && standPosition)
        {
            yHeadPosition = yPositionWanted;
        }

        _head.Position = new Vector3(_head.Position.X, yHeadPosition, _head.Position.Z);
    }

    public void LerpNeck(float yRotationWanted, float delta)
    {
        float yNeckRotation = _neck.Rotation.Y;
        //float yCameraRotation = _neck.Rotation.Y;
        yNeckRotation = Mathf.Lerp(yNeckRotation, yRotationWanted, delta * _lerpMovementSensitivity);
        //yCameraRotation = Mathf.Lerp(yCameraRotation, yNeckRotation, delta * _lerpMovementSensitivity);
        _neck.Rotation = new Vector3(_neck.Rotation.X, yNeckRotation, _head.Position.Z);

        Vector3 finalRotation = new Vector3(_neck.Rotation.X, yRotationWanted, _neck.Rotation.Z);
        if(_neck.Rotation.IsEqualApprox(finalRotation))
        {
            _neck.Rotation = finalRotation;
        }

        _camera.Rotation = new Vector3(_camera.Rotation.X, _neck.Rotation.Y, _camera.Rotation.Z);
    }

    
    public Vector3 LerpVector(Vector3 vector, string axis, float to, float delta)
    {
        float from;
        switch(axis)
        {
            case "x":
                from = vector.X;
                break;
            case "y":
                from = vector.Y;
                break;
            case "z":
                from = vector.Z;
                break;
            default:
                return Vector3.Zero;
        }

        from = Mathf.Lerp(from, to, delta * _lerpMovementSensitivity);
        
        Vector3 resultVector = Vector3.Zero;
        Vector3 finalVector = Vector3.Zero;

        switch(axis)
        {
            case "x":
                resultVector = new Vector3(from, vector.Y, vector.Z);
                finalVector = new Vector3(to, vector.Y, vector.Z);
                break;
            case "y":
                resultVector = new Vector3(vector.X, from, vector.Z);
                finalVector = new Vector3(vector.X, to, vector.Z);
                break;
            case "z":
                resultVector = new Vector3(vector.X, vector.Y, from);
                finalVector = new Vector3(vector.X, vector.Y, to);
                break;
        }

        if(resultVector.IsEqualApprox(finalVector))
        {
            resultVector = finalVector;
        }

        return resultVector;
    }

    private void LerpDirection(Vector3 finalDirection, float delta)
    {
        float xDirection = Mathf.Lerp(_direction.X, finalDirection.X, (float)delta * _lerpMovementSensitivity);
        float zDirection = Mathf.Lerp(_direction.Z, finalDirection.Z, (float)delta * _lerpMovementSensitivity);
        _direction = new Vector3(xDirection, 0, zDirection);

        if(_direction.IsEqualApprox(finalDirection))
        {
            _direction = finalDirection;
        }
    }

    private void RotateCamera(InputEventMouseMotion mouseMotion)
    {
        RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _mouseSensitivity));
        _head.RotateX(Mathf.DegToRad(-mouseMotion.Relative.Y * _mouseSensitivity));
        float xHeadRotation = Mathf.Clamp(_head.Rotation.X, Mathf.DegToRad(-89), Mathf.DegToRad(89));
        _head.Rotation = new Vector3(xHeadRotation, _head.Rotation.Y, _head.Rotation.Z);
    }

    private void FreelookCamera(InputEventMouseMotion mouseMotion)
    {
        _neck.RotateY(Mathf.DegToRad(-mouseMotion.Relative.X * _mouseSensitivity));
        float yNeckRotation = Mathf.Clamp(_neck.Rotation.Y, Mathf.DegToRad(-60), Mathf.DegToRad(60));
        _neck.Rotation = new Vector3(_neck.Rotation.X, yNeckRotation, _neck.Rotation.Z);

        float yCameraRotation = Mathf.DegToRad(_neck.Rotation.Y * _freeLookTiltAmount);
        yCameraRotation = Mathf.Clamp(yCameraRotation, Mathf.DegToRad(-80), Mathf.DegToRad(80));
        _camera.Rotation = new Vector3(_camera.Rotation.X, yCameraRotation, _camera.Rotation.Z);
    }

    public bool IsFloorCheckerColliding()
    {
        return _floorChecker.IsColliding();
    }

    public bool IsWallCheckerColliding()
    {
        return _wallChecker.IsColliding();
    }
}