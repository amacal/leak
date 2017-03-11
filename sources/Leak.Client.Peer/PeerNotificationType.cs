namespace Leak.Client.Peer
{
    public enum PeerNotificationType
    {
        Disconnected,
        BitfieldChanged,
        StatusChanged,
        BlockRequested,
        BlockReceived,
        MetafileMeasured,
        MetafileReceived,
        MetafileCompleted
    }
}