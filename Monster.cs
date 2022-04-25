using Godot;
using System;
using System.Collections.Generic;
using Libplanet;
using Script.Action;

public class Monster : Area2D
{
    [Signal]
    public delegate void ClickSignal(ActionWrapper action);

    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private Dictionary<Address, int> _attacks = new Dictionary<Address, int>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    //  // Called every frame. 'delta' is the elapsed time since the previous frame.
    //  public override void _Process(float delta)
    //  {
    //
    //  }

    public override void _InputEvent(
        Godot.Object viewport, InputEvent @event, int shapeIdx)
    {
        base._InputEvent(viewport, @event, shapeIdx);
        if (@event is InputEventMouseButton iemb
            && iemb.Pressed
            && (ButtonList)iemb.ButtonIndex == ButtonList.Left)
        {
            GD.Print($"Monster clicked");
            ActionWrapper action = new ActionWrapper(new AddCount());
            EmitSignal(nameof(ClickSignal), action);
        }
    }
}
