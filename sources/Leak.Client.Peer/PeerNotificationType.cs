namespace Leak.Client.Peer
{
    public enum PeerNotificationType
    {
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