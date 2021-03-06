﻿#region

using LoESoft.Core.config;
using LoESoft.GameServer.realm;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using static LoESoft.GameServer.networking.Client;

#endregion

namespace LoESoft.GameServer.networking
{
    internal class Server
    {
        public Server(RealmManager manager)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                NoDelay = true,
                UseOnlyOverlappedIO = true
            };
            Manager = manager;
        }

        private Socket Socket { get; set; }

        public RealmManager Manager { get; private set; }

        public void Start()
        {
            try
            {
                Socket.Bind(new IPEndPoint(IPAddress.Any, Settings.GAMESERVER.PORT));
                Socket.Listen(0xFF);
                Socket.BeginAccept(Listen, null);
            }
            catch (Exception ex)
            { GameServer.ForceShutdown(ex); }
        }

        private void Listen(IAsyncResult ar)
        {
            Socket skt = null;

            try
            { skt = Socket.EndAccept(ar); }
            catch (ObjectDisposedException) { }

            try
            { Socket.BeginAccept(Listen, null); }
            catch (ObjectDisposedException) { }

            if (skt != null)
                new Client(Manager, skt);
        }

        public void Stop()
        {
            foreach (ClientData cData in Manager.ClientManager.Values.ToArray())
            {
                cData.Client.Save();
                Manager.TryDisconnect(cData.Client, DisconnectReason.STOPING_SERVER);
            }

            Socket.Close();
        }
    }
}