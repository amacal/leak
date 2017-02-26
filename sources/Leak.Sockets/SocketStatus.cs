namespace Leak.Sockets
{
    public enum SocketStatus
    {
        OK = 0,
        OperationAborted = 995,
        NotSocket = 10038,
        ConnectionAborted = 10053,
        TimedOut = 10060,
        ConnectionRefused = 10061,
    }
}