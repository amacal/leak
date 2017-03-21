namespace Leak.Client
{
    public enum NotificationType
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
        DataChanged,
        PieceCompleted,
        PieceRejected,
        MemorySnapshot,
        ListenerStarted,
        ListenerFailed
    }
}