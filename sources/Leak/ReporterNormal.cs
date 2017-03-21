using System;
using System.IO;
using Leak.Client;
using Leak.Common;

namespace Leak
{
    public class ReporterNormal : Reporter
    {
        public bool Handle(Notification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.PeerConnected:
                    Console.WriteLine($"Peer: connected {notification.Peer}");
                    break;

                case NotificationType.PeerDisconnected:
                    Console.WriteLine($"Peer: disconnected {notification.Peer}");
                    break;

                case NotificationType.PeerRejected:
                    Console.WriteLine($"Peer: rejected {notification.Remote}");
                    break;

                case NotificationType.PeerBitfieldChanged:
                    Console.WriteLine($"Bitfield: {notification.Bitfield.Completed}/{notification.Bitfield.Length} pieces completed");
                    break;

                case NotificationType.PeerStatusChanged:
                    Console.WriteLine($"Status: {notification.State}");
                    break;

                case NotificationType.MetafileMeasured:
                    Console.WriteLine($"Metadata: {notification.Size}");
                    break;

                case NotificationType.MetafileRequested:
                    Console.WriteLine($"Metadata: requested piece {notification.Piece}");
                    break;

                case NotificationType.MetafileReceived:
                    Console.WriteLine($"Metadata: received piece {notification.Piece}");
                    break;

                case NotificationType.MetafileCompleted:

                    Console.WriteLine($"Metadata: {notification.Metainfo.Pieces.Length} pieces [{notification.Metainfo.Properties.PieceSize} bytes]");

                    foreach (MetainfoEntry entry in notification.Metainfo.Entries)
                    {
                        Console.WriteLine($"Metadata: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
                    }

                    break;

                case NotificationType.DataAllocated:
                    Console.WriteLine($"Data: allocated");
                    break;

                case NotificationType.DataVerified:
                    Console.WriteLine($"Data: verified {notification.Bitfield.Length} pieces");
                    break;

                case NotificationType.DataCompleted:
                    Console.WriteLine($"Data: completed");
                    return false;

                case NotificationType.PieceCompleted:
                    Console.WriteLine($"Data; completed piece {notification.Piece}");
                    break;

                case NotificationType.PieceRejected:
                    Console.WriteLine($"Data; rejected piece {notification.Piece}");
                    break;

                case NotificationType.MemorySnapshot:
                    Console.WriteLine($"Memory: snapshot {notification.Size}");
                    break;

                case NotificationType.ListenerStarted:
                    Console.WriteLine($"Listener: started on {notification.Remote}");
                    break;

                case NotificationType.ListenerFailed:
                    Console.WriteLine($"Listener: failed because '{notification.Description}'");
                    break;
            }

            return true;
        }
    }
}