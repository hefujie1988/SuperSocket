using System;
using System.Buffers;
using System.Threading.Tasks;
using System.Net.Sockets;
using SuperSocket.Channel;
using SuperSocket.ProtoBase;
using System.Threading;
using System.Net;

namespace SuperSocket.Udp
{
    class UdpPipeChannel<TPackageInfo> : VirtualChannel<TPackageInfo>, IChannelWithSessionIdentifier
    {
        private Socket _socket;

        private IPEndPoint _remoteEndPoint;

        public UdpPipeChannel(Socket socket, IPipelineFilter<TPackageInfo> pipelineFilter, ChannelOptions options, IPEndPoint remoteEndPoint, string sessionIndentifier)
            : base(pipelineFilter, options)
        {
            _socket = socket;
            _remoteEndPoint = remoteEndPoint;
            SessionIdentifier = sessionIndentifier;
        }

        public string SessionIdentifier { get; }

        protected override void Close()
        {
            
        }

        protected override ValueTask<int> FillPipeWithDataAsync(Memory<byte> memory, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        protected override async ValueTask<int> SendOverIOAsync(ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            var total = 0;

            foreach (var piece in buffer)
            {
                total += await _socket.SendToAsync(GetArrayByMemory<byte>(piece), SocketFlags.None, _remoteEndPoint);
            }

            return total;
        }
    }
}