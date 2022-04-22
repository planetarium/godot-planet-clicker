using Godot;
using System;
using System.Collections.Generic;
using Util;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Store;
using Libplanet.Store.Trie;

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

        string storePath = helper.GetStorePath();
        string stateStorePath = helper.GetStateStorePath();
        IStore store = new DefaultStore(storePath);
        IStateStore stateStore = new TrieStateStore(
            new DefaultKeyValueStore(stateStorePath));

        IBlockPolicy<PolymorphicAction<ActionBase>> blockPolicy
            = helper.GetBlockPolicy();
        IStagePolicy<PolymorphicAction<ActionBase>> stagePolicy
            = helper.GetStagePolicy();
        IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> renderers
            = helper.GetRenderers();
        BlockChain<PolymorphicAction<ActionBase>> blockChain
            = new BlockChain<PolymorphicAction<ActionBase>>(
                policy: blockPolicy,
                stagePolicy: stagePolicy,
                store: store,
                stateStore: stateStore,
                genesisBlock: genesis,
                renderers: renderers);
        GD.Print($"Loaded blockchain tip index: {blockChain.Tip.Index}");
        GD.Print($"Loaded blockchain tip hash: {blockChain.Tip.Hash}");

        string host = helper.GetHost();
        int port = helper.GetPort();
        IEnumerable<IceServer> iceServers = helper.GetIceServers();
        AppProtocolVersion appProtocolVersion
            = helper.GetAppProtocolVersion();
        IEnumerable<PublicKey> trustedAppProtocolVersionSigners
            = helper.GetTrustedAppProtocolVersionSigners();
        DifferentAppProtocolVersionEncountered differentAppProtocolVersionEncountered
            = helper.GetDifferentAppProtocolVersionEncountered();
        Swarm<PolymorphicAction<ActionBase>> swarm = new Swarm<PolymorphicAction<ActionBase>>(
                    blockChain: blockChain,
                    privateKey: privateKey,
                    host: host,
                    listenPort: port,
                    iceServers: iceServers,
                    appProtocolVersion: appProtocolVersion,
                    trustedAppProtocolVersionSigners: trustedAppProtocolVersionSigners,
                    differentAppProtocolVersionEncountered: differentAppProtocolVersionEncountered);
        GD.Print("Swarm instance created");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
