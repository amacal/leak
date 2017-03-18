namespace Leak.Client.Swarm
{
    public enum SwarmNotificationType
    {
        PeerConnected,
        PeerDisconnected,
        PeerRejected,
        PeerBitfieldChanged,
        PeerStatusChanged,
        MetafileMeasured,
        MetafileReceived,
        MetafileRequested,
        MetafileCompleted,
        DataAllocated,
        DataVerified,
        DataCompleted,
        PieceCompleted,
        PieceRejected,
        MemorySnapshot
    }
}