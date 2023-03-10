using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using fwRelik.SSHSetup.Structs;

namespace fwRelik.SSHSetup.Extensions
{
    /// <summary>
    /// Allows you to obtain information about the network state of this device and data for connecting to the device.
    /// </summary>
    public class NetworkInfo
    {
        /// <summary>
        /// Host name this device.
        /// </summary>
        public string HostName { get; private set; }

        /// <summary>
        /// The name of the current user.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// IP address belonging to this device.
        /// </summary>
        public IPAddress[] IpAddresses { get; private set; }

        /// <summary>
        /// The number of connections to this device.
        /// </summary>
        public int ConnectionCount { get; private set; } = 0;

        /// <summary>
        /// When creating the instance will initialize the state of the network.
        /// </summary>
        public NetworkInfo()
        {
            HostName = getHostName();
            UserName = getUserName();
            IpAddresses = getIpAddresses();
        }

        /// <summary>
        /// Checks network connectivity.
        /// </summary>
        /// <returns>
        /// Returns a boolean true if the connection is possible. Otherwise false.
        /// </returns>
        public bool GetNetworkConnectionStatus() => NetworkInterface.GetIsNetworkAvailable();


        /// <summary>
        /// Receives all connections to this device.
        /// </summary>
        /// <returns>Connections dictionary.</returns>
        /// <exception cref="ArgumentException"></exception>
        public Dictionary<int, ConnectionEntity> GetConnections()
        {
            try
            {
                int port = 22;
                var connectionEntities = new Dictionary<int, ConnectionEntity>();
                var properties = IPGlobalProperties.GetIPGlobalProperties();
                var connections = properties.GetActiveTcpConnections()
                    .Where(connection => connection.LocalEndPoint.Port == port).ToArray();

                for (int i = 0; i < connections.Length; i++)
                {
                    var connectionEntity = new ConnectionEntity
                    {
                        LocalAddress = connections[i].LocalEndPoint,
                        RemoteAddress = connections[i].RemoteEndPoint,
                        State = connections[i].State
                    };

                    connectionEntities.Add(i, connectionEntity);
                }

                ConnectionCount = connections.Length;

                return connectionEntities;
            }
            catch (Exception ex) { throw new ArgumentException(ex.Message); }
        }

        /// <summary>
        /// Will return host name.
        /// </summary>
        /// <returns>Host name.</returns>
        private string getHostName()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        /// Will return user name.
        /// </summary>
        /// <returns>User name.</returns>
        private string getUserName()
        {
            return Environment.UserName ?? "Undefined";
        }

        /// <summary>
        /// Will return all the IP addresses on this device.
        /// </summary>
        /// <returns>All IP addresses on this device.</returns>
        /// <exception cref="ArgumentException"></exception>
        private IPAddress[] getIpAddresses()
        {
            var hosts = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(item => item.AddressFamily == AddressFamily.InterNetwork)
                .ToArray();

            if (hosts != null) return hosts;
            throw new ArgumentException("No network adapters with an IPv4 address in the system");
        }
    }
}
