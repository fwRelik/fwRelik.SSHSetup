using System.Text;

namespace fwRelik.SSHSetup.Structs
{
    public struct ConnectionEntity
    {
        /// <summary>
        /// The local address of this device.
        /// </summary>
        public string LocalAddress;

        /// <summary>
        /// The local port of this device.
        /// </summary>
        public string LocalPort;

        /// <summary>
        /// Remote device address.
        /// </summary>
        public string RemoteAddress;

        /// <summary>
        /// Remote device port.
        /// </summary>
        public string RemotePort;

        /// <summary>
        /// Connection state.
        /// </summary>
        public string State;

        /// <summary>
        /// Applied settings for this connection.
        /// </summary>
        public string AppliedSetting;

        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            stringBuilder
                .Append($"LocalAddress: {LocalAddress}, ")
                .Append($"LocalPort: {LocalPort}, ")
                .Append($"RemoteAddress: {RemoteAddress}, ")
                .Append($"RemotePort: {RemotePort}, ")
                .Append($"State: {State}, ")
                .Append($"AppliedSetting: {AppliedSetting}, ");

            return stringBuilder.ToString();
        }
    }
}
