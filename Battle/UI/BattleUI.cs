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
    private Button _pressionAttackMenuButton;
    private Control _pressionAttackMenuUI;

    public bool CanSelect {set {_canSelect = value;} get {return _canSelect;}}

    public override void _Ready()
    {
        _actionGridContainer = GetNode<GridContainer>("ActionGridContainer");
        _battleDatabase = GetTree().Root.GetNode<BattleDatabase>("BattleDatabase");
        _oneMoreUI = GetNode<Label>("OneMoreUI");
        _oneMoreTimerUI = _oneMoreUI.GetNode<Timer>("OneMoreTimerUI");
        _pressionAttackMenuUI = GetNode<Control>("PressionAttackMenuUI");

        for(int x = 0; x < _actionGridContainer.GetChildCount(); x++)
        {
            if(_actionGridContainer.GetChild(x) is ActionButton actionButton)
            {
                actionButton.Setup(this);
            }
        }

        for(int x = 0; x < _pressionAttackMenuUI.GetChildCount(); x++)
        {
            if(_pressionAttackMenuUI.GetChild(x) is ActionButton actionButton)
            {
                actionButton.Setup(this);
            }
        }

        // Subscribing to events
        BattleManager.OnCurrentCharacterChanged += BattleManager_OnCurrentCharacterChanged;
        BattleManager.OnOneMore += BattleManager_OneMore;
        CharacterReceptorSelector.OnCharacterSelectorCanceled += CharacterReceptorSelector_OnCharacterSelectorCanceled;
        CharacterReceptorSelector.OnCharacterSelectorStarted += CharacterReceptorSelector_OnCharacterSelectorStarted;
        BaseAction.ActionTaken += BaseAction_ActionTaken;

        // Making the pression menu button funcional.
        _pressionAttackMenuButton = (Button)_actionGridContainer.GetChild(_actionGridContainer.GetChildCount()-1);
        _pressionAttackMenuButton.Pressed += () => {
            _pressionAttackMenuUI.Visible = !_pressionAttackMenuUI.Visible;
        };
    }

    private void BattleManager_OnCurrentCharacterChanged(object sender, BattleManager.OnCurrentCharacterChangedEventArgs e)
    {
        _canSelect = true;
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

    private void BaseAction_ActionTaken(object sender, EventArgs e)
    {
        // Disable all Battle Menus
        _pressionAttackMenuUI.Hide();
    }

    private void OnOneMoreTimeUITimeout()
    {
        _oneMoreUI.Hide();
    }
}
