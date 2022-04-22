using Godot;
using System;
using Util;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Crypto;

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

        InitHelper helper = new InitHelper();
        PrivateKey privateKey = helper.GetPrivateKey();
        Block<PolymorphicAction<ActionBase>> genesis = helper.GetGenesis();
        GD.Print(
            $"Loaded private key address: {new Address(privateKey.PublicKey).ToHex()}");
        GD.Print(
            $"Loaded genesis block hash: {genesis.Hash}");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
