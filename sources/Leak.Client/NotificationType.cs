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
        MetafileRequested,
        MetafileReceived,
        MetafileCompleted,
        DataAllocated,
        DataVerified,
        DataChanged,
        DataCompleted,
        PieceCompleted,
        PieceRejected,
        MemorySnapshot,
        ListenerStarted,
        ListenerFailed,
        TrackerAnnounced
    }
}