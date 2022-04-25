using System;
using Libplanet;

namespace Script.State
{
    [Serializable]
    public abstract class State
    {
        public Address address;

        protected State(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            this.address = address;
        }
    }
}
