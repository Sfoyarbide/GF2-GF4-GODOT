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
        _battleDatabase.BattleManager.OnTurnEnd += BattleManager_OnTurnEnd;
        _battleDatabase.BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        _battleDatabase.CharacterReceptorSelector.OnCharacterSelectorStarted += CharacterReceptorSelector_OnCharacterSelectorStarted;
        _battleDatabase.CharacterReceptorSelector.OnCharacterSelectorCanceled += CharacterReceptorSelector_OnCharacterSelectorCanceled;
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
            _currentCamera.Current = false;
            _currentCamera = newCamera;
        }
    }

    private void UpdateCamera(Camera3D camera, Vector3 characterPosition, bool isOnlyLookAt=false, bool isOnlyFollow=false)
    {
        // Updates a camera to their target Transform.
        if(!isOnlyFollow) // if is only follow then, we won't change the lookAt.
        {
            camera.LookAt(characterPosition);
        }
        if(!isOnlyLookAt) // if is only lookAt then, we won't change the follow.
        {
            float distanceFromCharacterZ = 2;
            if(characterPosition.Z < 0)
            {
                distanceFromCharacterZ = -distanceFromCharacterZ;
            }
            camera.Position = characterPosition + Vector3.Forward * distanceFromCharacterZ + Vector3.Up;
        }
        
        //_currentCamera = camera;
    }

    private void UpdateCurrentCharacterCamera() // Updates the CharacterSelectedCamera to his position.
    {
        Character currentCharacter = _battleDatabase.BattleManager.GetCurrentCharacter();
        Vector3 currentCharacterPosition = currentCharacter.Position;
        UpdateCamera(_currentCharacterCamera, currentCharacterPosition);
        UpdateCamera(_characterSelectorCamera, currentCharacterPosition, false, true); // Update the characterSelectingReceptorCamera follow
    }

    private void UpdateCharacterSelectorCamera() // Updates the CharacterSelectingReceptorCamera.
    {
        Character currentCharacterReceptor = _battleDatabase.CharacterReceptorSelector.GetCharacterReceptor();
        Vector3 currentCharacterReceptorPosition = currentCharacterReceptor.Position;
        UpdateCamera(_characterSelectorCamera, currentCharacterReceptorPosition, true);
    }

    private void UpdateCharacterSkillCamera() // Updates the CharacterSkillActionCamera.
    {
        Character currentCharacter = _battleDatabase.BattleManager.GetCurrentCharacter();
        Vector3 currentCharacterPosition = currentCharacter.Position;
        UpdateCamera(_skillCamera, currentCharacterPosition);
    }

    private void SetAllCamerasToState(bool state, Camera3D notThisCamera=null)
    {
        if(notThisCamera != _currentCamera)
        {
            _currentCharacterCamera.Current = false;
        }
        if(notThisCamera != _characterSelectorCamera)
        {
            _characterSelectorCamera.Current = false;
        }
        if(notThisCamera != _skillCamera)
        {
            _skillCamera.Current = false;
        }
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
        UpdateCurrentCharacterCamera();
    }

    private void BattleManager_OnActionExecute(object sender, BattleManager.OnActionExecuteEventArgs e)
    {
        // Checks the selected action, and based on that we can set the proper camera.
        BaseAction baseAction = _battleDatabase.BattleManager.GetCurrentCharacter().DataContainer.SelectedAction;
        switch(baseAction)
        {
            case AttackAction attackAction:
                ChangeCamera(_currentCharacterCamera, false);
                UpdateCurrentCharacterCamera();
                break;
            case SkillAction skillAction:
                ChangeCamera(_characterSelectorCamera, false);
                UpdateCharacterSkillCamera();

                /*
                OnActionIsSkill?.Invoke(this, new OnActionIsSkillEventArgs{
                    skill = skillAction.GetCurrentSkill()
                });
                */
                
                break;
        }
    }
    
    private void BattleManager_OnTurnEnd(object sender, EventArgs e)
    {
        ChangeCamera(_currentCharacterCamera, false);
    }
}