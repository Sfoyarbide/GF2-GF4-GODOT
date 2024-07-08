using Godot;
using System;

public partial class BattleCameraManager : Node
{
    private BattleDatabase _battleDatabase;
    private Camera3D _currentCharacterCamera;
    private Camera3D _characterSelectorCamera;
    private Camera3D _skillCamera;
    private Camera3D _currentCamera;
    
    public static event EventHandler<CameraChangedEventArgs> CameraChanged;
    public class CameraChangedEventArgs : EventArgs
    {
        public Camera3D from;
        public Camera3D to;
        public float transitionDuration;
    }
    
    public override void _Ready()
    {
        _currentCharacterCamera = GetChild<Camera3D>(0);
        _characterSelectorCamera = GetChild<Camera3D>(1);
        _skillCamera = GetChild<Camera3D>(2); 
        _currentCamera = _currentCharacterCamera;

        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        BattleManager.OnBattleStart += BattleManager_OnBattleStart;
        BattleManager.OnBattleEnd += BattleManager_OnBattleEnd;
        BattleManager.OnTurnEnd += BattleManager_OnTurnEnd;
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        CharacterReceptorSelector.OnCharacterSelectorStarted += CharacterReceptorSelector_OnCharacterSelectorStarted;
        CharacterReceptorSelector.OnCharacterSelectorCanceled += CharacterReceptorSelector_OnCharacterSelectorCanceled;
        CharacterReceptorSelector.OnCharacterReceptorSelected += CharacterReceptorSelector_OnCharacterReceptorSelected;
    }

    private void ChangeCamera(Camera3D newCamera, bool isTransitioning, float duration = 1f)
    {
        if(isTransitioning)
        {
            CameraChangedEventArgs e = new CameraChangedEventArgs
            {
                from = _currentCamera,
                to = newCamera,
                transitionDuration = duration
            };
            CameraChanged?.Invoke(this, e);
            _currentCamera = newCamera;
        }
        else
        {
            newCamera.Current = true;
            //_currentCamera.Current = false;
            _currentCamera = newCamera;
        }
    }

    private void UpdateCamera(Camera3D camera, Transform3D newCameraTransform, Vector3 characterPosition, bool isOnlyLookAt=false, bool isOnlyFollow=false)
    {
        // Updates a camera to their target Transform.
        if(!isOnlyFollow) // if is only follow then, we won't change the lookAt.
        {
            camera.LookAt(characterPosition);
        }
        if(!isOnlyLookAt) // if is only lookAt then, we won't change the follow.
        {
            camera.GlobalTransform = newCameraTransform;
        }
        
        //_currentCamera = camera;
    }

    private void UpdateCurrentCharacterCamera() // Updates the CharacterSelectedCamera to his position.
    {
        Character currentCharacter = _battleDatabase.BattleManager.GetCurrentCharacter();
        Vector3 currentCharacterPosition = currentCharacter.GlobalPosition;
        _currentCharacterCamera.GlobalTransform = currentCharacter.GetMarkerChildTransform(0);
        UpdateCamera(_currentCharacterCamera, currentCharacter.GetMarkerChildTransform(0), currentCharacterPosition);
        UpdateCamera(_characterSelectorCamera, currentCharacter.GetMarkerChildTransform(1), currentCharacterPosition, false, true); // Update the characterSelectingReceptorCamera follow
    }

    private void UpdateCharacterSelectorCamera() // Updates the CharacterSelectingReceptorCamera.
    {
        Character currentCharacterReceptor = _battleDatabase.CharacterReceptorSelector.GetCharacterReceptor();
        Vector3 currentCharacterReceptorPosition = currentCharacterReceptor.GlobalPosition;
        UpdateCamera(_characterSelectorCamera, currentCharacterReceptor.GetMarkerChildTransform(1), currentCharacterReceptorPosition, true);
    }

    private void UpdateCharacterSkillCamera() // Updates the CharacterSkillActionCamera.
    {
        Character currentCharacter = _battleDatabase.BattleManager.GetCurrentCharacter();
        Vector3 currentCharacterPosition = currentCharacter.Position;
        UpdateCamera(_skillCamera, currentCharacter.GetMarkerChildTransform(0), currentCharacterPosition);
    }

    private void CharacterReceptorSelector_OnCharacterSelectorStarted(object sender, EventArgs e)
    {
        // If the SelectedCharacterReceptor stats his selection then we set the CharacterSelectingReceptorCamera and update it. 
        UpdateCharacterSelectorCamera();
        ChangeCamera(_characterSelectorCamera, true);
    }

    private void CharacterReceptorSelector_OnCharacterReceptorSelected(object sender, CharacterReceptorSelector.OnCharacterReceptorSelectedEventArgs e)
    {
        // When SelectedCharacterReceptor changes, it updates CharacterSelectorCamera.
        UpdateCharacterSelectorCamera();
    }

    private void CharacterReceptorSelector_OnCharacterSelectorCanceled(object sender, EventArgs e)
    {
        ChangeCamera(_currentCharacterCamera, true);
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        // Updates the camera if the character changes.
        if(e.inCombat)
        {
            UpdateCurrentCharacterCamera();
        }
    }

    private void BattleManager_OnBattleStart(object sender, BattleManager.OnBattleStartEventArgs e)
    {
        _currentCharacterCamera.Current = true;
    }

    private void BattleManager_OnBattleEnd(object sender, BattleManager.OnBattleEndEventArgs e)
    {
        _currentCharacterCamera.Current = false;
    }
    
    private void BattleManager_OnTurnEnd(object sender, EventArgs e)
    {
        ChangeCamera(_currentCharacterCamera, false);
    }
}