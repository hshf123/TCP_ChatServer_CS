using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(4096);
        object _lock = new object();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisConnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);

            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        void Disconnect()
        {
            if (Interlocked.CompareExchange(ref _disconnected, 1, 0) == 1)
                return;
            OnDisConnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        void RegisterRecv()
        {
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> seg = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(seg.Array, seg.Offset, seg.Count);

            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (pending == false)
                OnRecvCompleted(null, _recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
            {
                Disconnect();
                return;
            }

            int processLen = OnRecv(_recvBuffer.ReadSegment);
            if (processLen < 0 || _recvBuffer.DataSize < processLen)
            {
                Disconnect();
                return;
            }

            if (_recvBuffer.OnRead(args.BytesTransferred) == false)
            {
                Disconnect();
                return;
            }

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                {
                    RegisterSend();
                }
            }
        }

        void RegisterSend()
        {
            _pendingList.Clear();

            while (_sendQueue.Count > 0)
            {
                byte[] sendBuff = _sendQueue.Dequeue();
                ArraySegment<byte> segment = new ArraySegment<byte>(sendBuff, 0, sendBuff.Length);
                _pendingList.Add(segment);
            }
            _sendArgs.BufferList = _pendingList;
            bool pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
                {
                    OnSend(args.BytesTransferred);
                    _sendArgs.BufferList = null;
                    _pendingList.Clear();

                    if (_sendQueue.Count > 0)
                        RegisterSend();
                }
                else
                {
                    Console.WriteLine($"OnSendCompleted Fail {args.SocketError}");
                }
            }
        }
    }
}
