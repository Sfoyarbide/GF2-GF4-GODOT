using Godot;
using System;

public partial class HpChangedUIManager : Node
{
    [Export]
    private PackedScene _HpChangedUIScene;
    private Node3D _battleNode;

    public override void _Ready()
    {
        CharacterDataResource.OnHpChanged += CharacterDataResource_OnHpChanged;
        _battleNode = (Node3D)GetParent().GetParent();
    }

    private void CharacterDataResource_OnHpChanged(object sender, CharacterDataResource.OnHpChangedEventArgs e)
    {
        ShowHpChangedUI(e.character, e.difference, e.isLessThanBefore);
    }

    public void ShowHpChangedUI(Character character, int hpChanged, bool isLessThanBefore)
    {
        HpChangedUI hpChangedUI = (HpChangedUI)_HpChangedUIScene.Instantiate();
        _battleNode.AddChild(hpChangedUI);
    
        hpChangedUI.Setup(hpChanged, character.GetMarkerChildTransform(3), isLessThanBefore);
    }

}