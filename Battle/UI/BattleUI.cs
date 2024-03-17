using Godot;
using System;

public partial class BattleUI : Control
{
    private int _actionIndex;
    private int _subMenuIndex;
    [Export]
    private bool _canSelect;
    private GridContainer _actionGridContainer;
    [Export]
    private PackedScene _actionButtonScene;
    private BattleDatabase _battleDatabase;
    private Label _oneMoreUI;
    private Timer _oneMoreTimerUI;
    public static event EventHandler<OnActionSelectedChangedEventArgs> OnActionSelectedChanged;
    public class OnActionSelectedChangedEventArgs : EventArgs
    {
        public int actionIndex;
    }

    public override void _Ready()
    {
        _actionGridContainer = GetNode<GridContainer>("ActionGridContainer");
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        BattleManager.OnOneMore += BattleManager_OneMore;
        CharacterReceptorSelector.OnCharacterSelectorCanceled += CharacterReceptorSelector_OnCharacterSelectorCanceled;
        CharacterReceptorSelector.OnCharacterSelectorStarted += CharacterReceptorSelector_OnCharacterSelectorStarted;
        ActionButton.OnActionButtonDown += ActionButton_OnActionButtonDown;
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _oneMoreUI = GetNode<Label>("OneMoreUI");
        _oneMoreTimerUI = _oneMoreUI.GetNode<Timer>("OneMoreTimerUI");
    }

    public override void _Process(double delta)
    {
        if(!_canSelect)
        {
            return;
        }

        int previousActionIndex = _actionIndex;
        //_actionIndex = CombatCalculations.MoveTheIndex(0, _actionGridContainer.GetChildCount(), _actionIndex);
        if(_actionIndex != previousActionIndex)
        {
            OnActionSelectedChanged?.Invoke(this, new OnActionSelectedChangedEventArgs{
                actionIndex = _actionIndex
            });
        }
    }

    private void CreateActionButtons(Character currentCharacter)
    {
        //int index = 0;
        /*
        foreach(BaseAction baseAction in currentCharacter.DataContainer.ActionList)
        {
            ActionButton actionButton = (ActionButton)_actionButtonScene.Instantiate();
            _actionGridContainer.AddChild(actionButton);
            actionButton.SetupActionButton(index, baseAction.GetActionName());
            index++;
        }
        */
        for(int x = 0; x < currentCharacter.DataContainer.ActionList.Count; x++)
        {
            ActionButton actionButton = (ActionButton)_actionButtonScene.Instantiate();
            _actionGridContainer.AddChild(actionButton);
            actionButton.SetupActionButton(x, currentCharacter.DataContainer.ActionList[x].GetActionName());
        }
    }

    private void DeleteActionButtons()
    {
        for(int x = 0; x < _actionGridContainer.GetChildCount(); x++)
        {
            _actionGridContainer.GetChild(x).QueueFree();
        }
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _canSelect = true;
        DeleteActionButtons();
        CreateActionButtons(e.currentCharacter);
    }

    private void ActionButton_OnActionButtonDown(object sender, ActionButton.OnActionButtonDownEventArgs e)
    {
        if(_canSelect == true)
        {
            _actionIndex = e.selectedActionIndex;
            OnActionSelectedChanged?.Invoke(this, new OnActionSelectedChangedEventArgs{
                actionIndex = _actionIndex
            });
        }
    }

    private void CharacterReceptorSelector_OnCharacterSelectorCanceled(object sender, EventArgs e)
    {
        _canSelect = true;
    }

    private void CharacterReceptorSelector_OnCharacterSelectorStarted(object sender, EventArgs e)
    {
        _canSelect = false;
    }

    private void BattleManager_OneMore(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _oneMoreUI.Show();
        _oneMoreTimerUI.Start();
    }

    private void OnOneMoreTimeUITimeout()
    {
        _oneMoreUI.Hide();
    }
}
