using Godot;

namespace Util
{
    public static class FileManager
    {
        public static readonly string ResourcePath
            = ProjectSettings.GlobalizePath("res://");

        public static readonly string UserPath
            = ProjectSettings.GlobalizePath("user://");

        public static readonly string GenesisBlockPath
            = System.IO.Path.Combine(ResourcePath, "genesis");

        public static readonly string PrivateKeyPath
            = System.IO.Path.Combine(UserPath, "private_key");

        public static readonly string StorePath
            = System.IO.Path.Combine(UserPath, "store");

        public static readonly string StateStorePath
            = System.IO.Path.Combine(UserPath, "state_store");
    }
}
