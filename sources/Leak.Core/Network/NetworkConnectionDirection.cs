namespace Leak.Core.Network
{
    /// <summary>
    /// Indicates who is the initiator of the connection.
    /// </summary>
    public enum NetworkConnectionDirection
    {
        /// <summary>
        /// The connection was initiated by remote endpoint.
        /// </summary>
        Incoming,

        /// <summary>
        /// The connection was initiated by local endpoint.
        /// </summary>
        Outgoing
    }
}