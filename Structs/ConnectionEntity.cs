using System.Net;
using System.Text;
using System.Net.NetworkInformation;

namespace fwRelik.SSHSetup.Structs
{
    /// <summary>
    /// Represents a connection entity.
    /// </summary>
    public struct ConnectionEntity
    {
        /// <summary>
        /// The local address of this device.
        /// </summary>
        public IPEndPoint LocalAddress; 

        /// <summary>
        /// Remote device address.
        /// </summary>
        public IPEndPoint RemoteAddress; 

        /// <summary>
        /// Connection state.
        /// </summary>
        public TcpState State;

        /// <summary>
        /// Returns a string with the fields LocalAddress, RemoteAddress and State.
        /// </summary>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder
                .Append($"LocalAddress: {LocalAddress}, ")
                .Append($"RemoteAddress: {RemoteAddress}, ")
                .Append($"State: {State}, ");

            return stringBuilder.ToString();
        }
    }
}
