namespace Leak.Client.Swarm
{
    public enum SwarmNotificationType
    {
        PeerConnected,
        PeerDisconnected,
        PeerBitfieldChanged,
        PeerStatusChanged,
        MetafileMeasured,
        MetafileReceived,
        MetafileRequested,
        MetafileCompleted,
        DataAllocated,
        DataVerified,
        DataCompleted,
        PieceCompleted
    }
}