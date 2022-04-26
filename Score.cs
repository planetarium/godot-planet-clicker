using Godot;
using System;
using System.Numerics;

public class Score : Label
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";
    private BigInteger _score;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _score = BigInteger.Zero;
        Align = AlignEnum.Center;
        Valign = VAlign.Center;;
        Text = $"Clicks: {_score}";
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }

    public void Update(BigInteger score)
    {
        Text = $"Clicks: {score}";
    }
}
