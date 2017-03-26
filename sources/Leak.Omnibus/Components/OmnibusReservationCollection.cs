using System;
using System.Collections.Generic;
using System.Linq;
using Leak.Common;

namespace Leak.Data.Map.Components
{
    public class OmnibusReservationCollection
    {
        private readonly TimeSpan lease;
        private readonly OmnibusReservationComparer comparer;

        private readonly Dictionary<BlockIndex, OmnibusReservation> byBlock;
        private readonly Dictionary<PeerHash, HashSet<OmnibusReservation>> byPeer;
        private readonly Dictionary<PieceInfo, HashSet<OmnibusReservation>> byPiece;

        public OmnibusReservationCollection(TimeSpan lease)
        {
            this.lease = lease;

            byPeer = new Dictionary<PeerHash, HashSet<OmnibusReservation>>();
            comparer = new OmnibusReservationComparer();

            byBlock = new Dictionary<BlockIndex, OmnibusReservation>(comparer);
            byPiece = new Dictionary<PieceInfo, HashSet<OmnibusReservation>>(comparer);
        }

        public int Count(DateTime now)
        {
            return byBlock.Values.Count(x => x.Expires > now);
        }

        public bool Contains(BlockIndex request, DateTime now)
        {
            OmnibusReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Expires > now;
        }

        public bool Contains(BlockIndex request, PeerHash peer)
        {
            OmnibusReservation book;

            if (byBlock.TryGetValue(request, out book) == false)
                return false;

            return book.Peer.Equals(peer);
        }

        public PeerHash Add(PeerHash peer, BlockIndex request, DateTime now)
        {
            PeerHash previous = null;
            OmnibusReservation reservation;

            HashSet<OmnibusReservation> forPeer;
            HashSet<OmnibusReservation> forPiece;

            if (byBlock.TryGetValue(request, out reservation))
            {
                previous = reservation.Peer;
            }

            reservation = new OmnibusReservation
            {
                Peer = peer,
                Expires = now + lease,
                Request = request
            };

            if (byPeer.TryGetValue(peer, out forPeer) == false)
            {
                forPeer = new HashSet<OmnibusReservation>();
                byPeer.Add(peer, forPeer);
            }

            if (byPiece.TryGetValue(request.Piece, out forPiece) == false)
            {
                forPiece = new HashSet<OmnibusReservation>();
                byPiece.Add(request.Piece, forPiece);
            }

            byBlock[request] = reservation;
            forPeer.Add(reservation);
            forPiece.Add(reservation);

            return previous;
        }

        public int Complete(BlockIndex request, out PeerHash peer)
        {
            OmnibusReservation reservation;
            byBlock.TryGetValue(request, out reservation);

            if (reservation != null)
            {
                HashSet<OmnibusReservation> forPeer;
                HashSet<OmnibusReservation> forPiece;

                byPeer.TryGetValue(reservation.Peer, out forPeer);
                byPiece.TryGetValue(reservation.Request.Piece, out forPiece);

                forPeer.Remove(reservation);
                forPiece.Remove(reservation);
                byBlock.Remove(request);

                peer = reservation.Peer;
                return forPeer.Count;
            }

            peer = null;
            return 0;
        }

        public void Complete(PieceInfo piece, out ICollection<PeerHash> involved)
        {
            HashSet<OmnibusReservation> reservations;
            byPiece.TryGetValue(piece, out reservations);

            if (reservations != null)
            {
                involved = new HashSet<PeerHash>();

                foreach (OmnibusReservation reservation in reservations)
                {
                    involved.Add(reservation.Peer);
                }

                byPiece.Remove(piece);
            }
            else
            {
                involved = new PeerHash[0];
            }
        }

        public int Count(PeerHash peer)
        {
            HashSet<OmnibusReservation> books;
            byPeer.TryGetValue(peer, out books);

            if (books == null)
                return 0;

            return books.Count;
        }
    }
}