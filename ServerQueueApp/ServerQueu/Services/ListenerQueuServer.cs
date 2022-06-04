﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TresEnRayaApp
{
    public class ListenerQueuServer
    {
        private readonly string Ip;
        private readonly int Port;
        private readonly int Backlog;
        private bool Finish;

        private TcpListener? _tcpSocketServer = null;
        public TcpListener? TcpSocketServer { get { return _tcpSocketServer; } }

        private Thread? _threadListener = null;
        public Thread? ThreadListener
        {
            get { return _threadListener; }
        }

        private HandlerSessionListener? HandlerSessionListener=null;

        

        public ListenerQueuServer(string ip,int port,int backlog, HandlerSessionListener handlerSessionListener)
        {
            this.Ip = ip;
            this.Port = port;
            this.Backlog = backlog;
            HandlerSessionListener=handlerSessionListener;
        }

        public void RunThreads()
        {
            EnabledTcpSocketServerToStart();
            
            _threadListener= new Thread(() =>
            {
                if (_tcpSocketServer != null&&HandlerSessionListener!=null)
                {
                    while (_tcpSocketServer.Server.IsBound&& !Finish)
                    {
                        TcpClient tcpClient= _tcpSocketServer.AcceptTcpClient();
                        HandlerSessionListener.AddClient(tcpClient);
                    }
                    _tcpSocketServer.Stop();
                }

            });

            _threadListener.Start();
        }
        public void Close()
        {
            Finish=true;
        }

        private void EnabledTcpSocketServerToStart()
        {
            _tcpSocketServer=new TcpListener(System.Net.IPAddress.Parse(Ip),Port);
            _tcpSocketServer.Start(Backlog);
            Finish=false;
        }

    }
}