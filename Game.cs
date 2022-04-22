using Godot;
using System;
using Util;

public class Game : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Game started");
        GD.Print($"Genesis block path: {FileManager.GenesisBlockPath}");
        GD.Print($"Private key path: {FileManager.PrivateKeyPath}");
        GD.Print($"Store path: {FileManager.StorePath}");
        GD.Print($"State store path: {FileManager.StateStorePath}");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
