using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            // 1. 일단 Receive예약 신청
            // 2. 바로되면 완료
            // 3. 안되면 대기 했다가 완료.

            _socket = socket;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            args.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecv(args);
        }

        void Disconnect()
        {
            if (Interlocked.CompareExchange(ref _disconnected, 1, 0) == 1)
                return;
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = _socket.ReceiveAsync(args);
            if (pending == false)
            {
                OnRecvCompleted(null, args);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine(recvData);

                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            else
            {
                Disconnect();
            }
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff);
        }
    }
}
