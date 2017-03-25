using System;
using System.IO;
using System.Text;
using Leak.Common;

namespace Leak.Client.Notifications
{
    public class MetafileCompletedNotification : Notification
    {
        private readonly Metainfo metainfo;

        public MetafileCompletedNotification(Metainfo metainfo)
        {
            this.metainfo = metainfo;
        }

        public override NotificationType Type
        {
            get { return NotificationType.MetafileCompleted; }
        }

        public override void Dispatch(NotificationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Meta: pieces={Metainfo.Pieces.Length}; piece-size={PieceSize}");

            foreach (MetainfoEntry entry in Metainfo.Entries)
            {
                builder.AppendLine($"Meta: {String.Join(Path.DirectorySeparatorChar.ToString(), entry.Name)} [{entry.Size} bytes]");
            }

            return builder.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }

        public Metainfo Metainfo
        {
            get { return metainfo; }
        }

        public Size PieceSize
        {
            get { return new Size(metainfo.Properties.PieceSize); }
        }
    }
}