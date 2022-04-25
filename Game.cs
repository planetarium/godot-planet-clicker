using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Util;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Blockchain;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Net.Protocols;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Tx;

public class Game : Node2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override async void _Ready()
    {
        GD.Print("Game started");
        GD.Print($"Genesis block path: {FileManager.GenesisBlockPath}");
        GD.Print($"Private key path: {FileManager.PrivateKeyPath}");
        GD.Print($"Store path: {FileManager.StorePath}");
        GD.Print($"State store path: {FileManager.StateStorePath}");

        InitHelper helper = new InitHelper();
        PrivateKey privateKey = helper.GetPrivateKey();
        Address address = new Address(privateKey.PublicKey);
        Block<PolymorphicAction<ActionBase>> genesis = helper.GetGenesis();
        GD.Print(
            $"Loaded private key address: {address}");
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

        GD.Print("Starting bootstrap");
        try
        {
            await swarm.BootstrapAsync(
                seedPeers: new List<BoundPeer>(),
                pingSeedTimeout: 1_000,
                findPeerTimeout: 1_000,
                depth: 2,
                cancellationToken: default);
        }
        catch (PeerDiscoveryException pde)
        {
            GD.PushError(pde.ToString());
        }
        GD.Print("Bootstrap completed");

        GD.Print("Starting preload");
        await swarm.PreloadAsync(
            dialTimeout: TimeSpan.FromMilliseconds(1000),
            progress: null,
            cancellationToken: default);
        GD.Print("Preload completed");

        GD.Print("Starting Swarm");
        _ = swarm.StartAsync(
            millisecondsDialTimeout: 1_000,
            millisecondsBroadcastBlockInterval: 10_000,
            millisecondsBroadcastTxInterval: 10_000,
            cancellationToken: default);

        GD.Print("Waiting for swarm running state");
        await swarm.WaitForRunningAsync();
        GD.Print(
            $"Swarm started with address {ByteUtil.Hex(privateKey.PublicKey.Format(true))}"
            + $",{host},{port}");

        while (true)
        {
            var txs = new HashSet<Transaction<PolymorphicAction<ActionBase>>>();

            Block<PolymorphicAction<ActionBase>> block = await blockChain.MineBlock(privateKey);
            GD.Print(
                $"Mined a block:");
            GD.Print($"\tindex: {block.Index}");
            GD.Print($"\thash: {block.Hash}");
            GD.Print($"\tdifficulty: {block.Difficulty}");
            GD.Print($"\ttotal difficulty: {block.TotalDifficulty}");
            GD.Print($"\ttransactions: [{string.Join(", ", block.Transactions.Select(tx => tx.Id.ToString()))}]");
            swarm.BroadcastBlock(block);
        }
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//
//  }
}
