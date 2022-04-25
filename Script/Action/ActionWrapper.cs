using Godot;

namespace Script.Action
{
    /// <summary>
    /// A wrapper to allow passing <see cref="ActionBase"/> as a signal
    /// parameter.
    /// </summary>
    public class ActionWrapper : Object
    {
        public ActionBase Action { get; }

        public ActionWrapper(ActionBase action)
        {
            Action = action;
        }
    }
}
