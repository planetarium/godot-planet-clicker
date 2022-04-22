using Godot;
using System;
using System.Security.Cryptography;
using Bencodex;
using Libplanet;
using Libplanet.Action;
using Libplanet.Blocks;
using Libplanet.Crypto;

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
    }
}
