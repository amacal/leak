using Leak.Common;

namespace Leak.Client.Notifications
{
    public class BitfieldChangedNotification : Notification
    {
        private readonly PeerHash peer;
        private readonly Bitfield bitfield;

        public BitfieldChangedNotification(PeerHash peer, Bitfield bitfield)
        {
            this.peer = peer;
            this.bitfield = bitfield;
        }

        public override NotificationType Type
        {
            get { return NotificationType.PeerBitfieldChanged; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            return $"Peer: id={Peer}; bitfield={Bitfield.Completed}/{Bitfield.Length}";
        }

        public PeerHash Peer
        {
            get { return peer; }
        }

        public Bitfield Bitfield
        {
            get { return bitfield; }
        }
    }
}