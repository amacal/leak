using System;
using System.Collections.Generic;
using System.IO;
using Leak.Client;
using Leak.Common;

namespace Leak
{
    public class ReporterCompact : Reporter
    {
        private readonly HashSet<PeerHash> peers;
        private readonly HashSet<PeerHash> seeds;

        private Metainfo metainfo;
        private int completed;
        private Size memory;

        public ReporterCompact()
        {
            peers = new HashSet<PeerHash>();
            seeds = new HashSet<PeerHash>();
            memory = new Size(0);
        }

        public bool Handle(Notification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.PeerConnected:
                    peers.Add(notification.Peer);
                    break;

                case NotificationType.PeerDisconnected:
                    peers.Remove(notification.Peer);
                    seeds.Remove(notification.Peer);
                    break;

                case NotificationType.PeerRejected:
                    break;

                case NotificationType.PeerBitfieldChanged:
                    break;

                case NotificationType.PeerStatusChanged:

                    if (notification.State.IsRemoteChokingLocal == false)
                        seeds.Add(notification.Peer);

                    break;

                case NotificationType.MetafileMeasured:
                    break;

                case NotificationType.MetafileRequested:
                    break;

                case NotificationType.MetafileReceived:
                    break;

                case NotificationType.MetafileCompleted:

                    Console.WriteLine();
                    Console.WriteLine($"Metadata: {notification.Metainfo.Pieces.Length} pieces [{notification.Metainfo.Properties.PieceSize} bytes]");

                    foreach (MetainfoEntry entry in notification.Metainfo.Entries)
                    {
                        Console.WriteLine($"Metadata: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
                    }

                    metainfo = notification.Metainfo;
                    break;

                case NotificationType.DataAllocated:
                    Console.WriteLine();
                    Console.WriteLine($"Data: allocated");
                    break;

                case NotificationType.DataVerified:
                    Console.WriteLine();
                    Console.WriteLine($"Data: verified {notification.Bitfield.Length} pieces");
                    break;

                case NotificationType.DataCompleted:
                    Console.WriteLine();
                    Console.WriteLine($"Data: completed");
                    return false;

                case NotificationType.DataChanged:
                    completed = notification.Completed;
                    break;

                case NotificationType.PieceCompleted:
                    break;

                case NotificationType.PieceRejected:
                    break;

                case NotificationType.MemorySnapshot:
                    memory = notification.Size;
                    break;

                case NotificationType.ListenerStarted:
                    break;

                case NotificationType.ListenerFailed:
                    break;
            }

            Console.Write($"\rData: peers {peers.Count}, seeds {seeds.Count}, completed {completed}, memory {memory}".PadRight(80));
            return true;
        }
    }
}