namespace Leak.Client.Peer
{
    public enum PeerNotificationType
    {
        PeerDisconnected,
        PeerBitfieldChanged,
        PeerStatusChanged,
        MetafileMeasured,
        MetafileReceived,
        MetafileCompleted,
        DataAllocated,
        DataVerified,
        DataCompleted,
        PieceCompleted
    }
}