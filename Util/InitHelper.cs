using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Bencodex;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Blockchain.Policies;
using Libplanet.Blockchain.Renderers;
using Libplanet.Crypto;
using Libplanet.Net;

namespace Util
{
    public class InitHelper
    {
        private static Codec _codec = new Codec();

        public static HashAlgorithmType HashAlgorithmType = HashAlgorithmType.Of<SHA256>();

        public static HashAlgorithmGetter HashAlgorithmGetter = blockIndex => HashAlgorithmType;

        public InitHelper()
        {
        }

        /// <summary>
        /// Either loads a saved <see cref="PrivateKey"/> if found
        /// at <see cref="FileManager.PrivateKeyPath"/> or generates a new one
        /// and saves at <see cref="FileManager.PrivateKeyPath"/>.
        /// </summary>
        public PrivateKey GetPrivateKey()
        {
            string path = FileManager.PrivateKeyPath;
            PrivateKey privateKey;
            if (System.IO.File.Exists(path))
            {
                GD.Print(
                    $"Private key found at {path}; attempting to load");
                string privateKeyHex = System.IO.File.ReadAllText(path);
                privateKey = new PrivateKey(ByteUtil.ParseHex(privateKeyHex));
                GD.Print("Private key successfully loaded");
            }
            else
            {
                GD.Print(
                    $"Private key not found at {path}; generating a new one");
                privateKey = new PrivateKey();
                string privateKeyHex = ByteUtil.Hex(privateKey.ByteArray);
                GD.Print("Private key successfully generated");
                System.IO.File.WriteAllText(path, privateKeyHex);
                GD.Print($"Private key saved to {path}");
            }

            return privateKey;
        }

        public Block<PolymorphicAction<ActionBase>> GetGenesis()
            => BlockMarshaler.UnmarshalBlock<PolymorphicAction<ActionBase>>(
                HashAlgorithmGetter,
                (Bencodex.Types.Dictionary)_codec.Decode(
                    System.IO.File.ReadAllBytes(FileManager.GenesisBlockPath)));

        public string GetStorePath()
            => FileManager.StorePath;

        public string GetStateStorePath()
            => FileManager.StateStorePath;

        public IBlockPolicy<PolymorphicAction<ActionBase>> GetBlockPolicy()
            => new BlockPolicy<PolymorphicAction<ActionBase>>(
                blockAction: null,
                blockInterval: TimeSpan.FromSeconds(10),
                difficultyStability: 1024,
                minimumDifficulty: 10_000);

        public IStagePolicy<PolymorphicAction<ActionBase>> GetStagePolicy()
            => new VolatileStagePolicy<PolymorphicAction<ActionBase>>();

        public IEnumerable<IRenderer<PolymorphicAction<ActionBase>>> GetRenderers()
        {
            return new List<IRenderer<PolymorphicAction<ActionBase>>>()
            {
                new AnonymousActionRenderer<PolymorphicAction<ActionBase>>()
                {
                    ActionRenderer = (action, ctx, nextStates) => { }
                }
            };
        }

        public string GetHost()
            => "0.0.0.0";

        public int GetPort()
            => 12345;

        public IEnumerable<IceServer> GetIceServers()
            => new List<IceServer>();

        public AppProtocolVersion GetAppProtocolVersion()
            => default;

        public IEnumerable<PublicKey> GetTrustedAppProtocolVersionSigners()
            => new List<PublicKey>();

        public DifferentAppProtocolVersionEncountered GetDifferentAppProtocolVersionEncountered()
            => (peer, peerVersion, localVersion)
                => GD.Print("Different app protocol version encountered");
    }
}
