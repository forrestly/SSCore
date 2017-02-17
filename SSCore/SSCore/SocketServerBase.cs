using SSCore.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SSCore
{
    public delegate void NewClientAcceptHandler(Socket client, object state);

    public class SocketServerBase
    {
        protected object SyncRoot = new object();

        //public IAppServer AppServer { get; private set; }

        public bool IsRunning { get; protected set; }

        //protected ListenerInfo[] ListenerInfos { get; private set; }

        //protected List<ISocketListener> Listeners { get; private set; }

        protected bool IsStopped { get; set; }

        private BufferManager _bufferManager;

        private ConcurrentStack<SocketAsyncEventArgsProxy> _readWritePool;

        public event NewClientAcceptHandler NewClientAccepted;

        private Socket _socket
        {
            get;
            set;
        }

        public SocketServerBase(Dictionary<string, string> config)
        {
            if (config == null)
                throw new ArgumentNullException();
            if (_socket != null)
                throw new Exception("socket server exists.");


            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(config["port"])));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Start()
        {
            int bufferSize = 0;//AppServer.Config.ReceiveBufferSize;

            int maxConnection = int.Parse(JsonConfigManager.Instance.Configures["max_connect"]);

            if (bufferSize <= 0)
                bufferSize = 1024 * 4;

            _bufferManager = new BufferManager(bufferSize * maxConnection, bufferSize);

            try
            {
                _bufferManager.InitBuffer();
            }
            catch (Exception e)
            {
                throw e;
                //AppServer.Logger.Error("Failed to allocate buffer for async socket communication, may because there is no enough memory, please decrease maxConnectionNumber in configuration!", e);
                //return false;
            }

            // preallocate pool of SocketAsyncEventArgs objects
            SocketAsyncEventArgs socketEventArg;

            var socketArgsProxyList = new List<SocketAsyncEventArgsProxy>(maxConnection);

            for (int i = 0; i < maxConnection; i++)
            {
                //Pre-allocate a set of reusable SocketAsyncEventArgs
                socketEventArg = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(socketEventArg);

                socketArgsProxyList.Add(new SocketAsyncEventArgsProxy(socketEventArg));
            }

            _readWritePool = new ConcurrentStack<SocketAsyncEventArgsProxy>(socketArgsProxyList);
            
            _socket.Listen(100);

            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();
            //m_AcceptSAE = acceptEventArg;
            acceptEventArg.Completed += AcceptEventArg_Completed;

            if (!_socket.AcceptAsync(acceptEventArg))
                ProcessAccept(acceptEventArg);

            return true;
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        void ProcessAccept(SocketAsyncEventArgs e)
        {
            Socket socket = null;

            if (e.SocketError != SocketError.Success)
            {
                var errorCode = (int)e.SocketError;

                //The listen socket was closed
                if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                    return;

                //OnError(new SocketException(errorCode));
            }
            else
            {
                socket = e.AcceptSocket;
            }

            e.AcceptSocket = null;

            bool willRaiseEvent = false;

            try
            {
                willRaiseEvent = _socket.AcceptAsync(e);
            }
            catch (ObjectDisposedException)
            {
                //The listener was stopped
                //Do nothing
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }
            catch (NullReferenceException)
            {
                //The listener was stopped
                //Do nothing
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }
            catch (Exception exc)
            {
                //OnError(exc);
                //make sure ProcessAccept won't be executed in this thread
                willRaiseEvent = true;
            }

            if (socket != null)
                OnNewClientAccepted(socket, null);

            //if (!willRaiseEvent)
            //    ProcessAccept(e);
        }

        protected virtual void OnNewClientAccepted(Socket socket, object state)
        {
            NewClientAccepted?.Invoke(socket, state);
        }


    }
}
