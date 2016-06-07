// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyConnectionCollection.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProxyConnectionCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;

    using global::Photon.SocketServer;

    public class ProxyConnectionCollection : PhotonConnectionCollection
    {
        public PhotonServerPeer LoginServer { get; protected set; }
        public PhotonServerPeer ChatServer { get; protected set; }
        public PhotonServerPeer WorldServer { get; protected set; }

        public ProxyConnectionCollection()
        {
            this.LoginServer = null;
            this.ChatServer = null;
            this.WorldServer = null;
        }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
            if (serverPeer.ServerId.HasValue)
            {
                // If server disconnects
                if (this.ChatServer != null && serverPeer.ServerId.Value == this.ChatServer.ServerId)
                {
                    this.ChatServer = null;
                }
                if (this.LoginServer != null && serverPeer.ServerId.Value == this.LoginServer.ServerId)
                {
                    this.LoginServer = null;
                }
                if (this.WorldServer != null && serverPeer.ServerId.Value == this.WorldServer.ServerId)
                {
                    this.WorldServer = null;
                }
            }
        }

        public override void Connect(PhotonServerPeer serverPeer)
        {
            if ((serverPeer.ServerType & (int)ServerType.Region) != 0)
            {
                var parameters = new Dictionary<byte, object>();

                var serverList = Servers.Where(
                    incomingSubServerPeer =>
                        incomingSubServerPeer.Value.ServerId.HasValue &&
                        !incomingSubServerPeer.Value.ServerId.Equals(serverPeer.ServerId)
                        && (incomingSubServerPeer.Value.ServerType & (int)ServerType.Region) != 0)
                    .ToDictionary(
                        incomingSubServerPeer =>
                        incomingSubServerPeer.Value.ApplicationName,
                        incomingSubServerPeer => incomingSubServerPeer.Value.TcpAddress);

                // Look up all the region servers
                if (serverList.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Sending list of {0} connected subservers", serverList.Count);
                    }
                    parameters.Add((byte)ServerParameterCode.SubServerDictionary, serverList);
                    serverPeer.SendEvent(new EventData((byte)ServerEventCode.SubServerList, parameters), new SendParameters());
                }
            }
        }

        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            // Client has an char id and login
            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                var para = new Dictionary<byte, object>
                               {
                                   {
                                       (byte)ClientParameterCode.CharacterId, clientPeer.ClientData<CharacterData>().CharacterId.Value
                                   },
                                   {
                                       (byte)ClientParameterCode.PeerId, clientPeer.PeerId
                                   }
                               };

                if (this.ChatServer != null)
                {
                    // Register with chat and region server
                    this.ChatServer.SendEvent(
                        new EventData((byte)ServerEventCode.CharacterRegister, para),
                        new SendParameters());
                }

                if (this.WorldServer != null)
                {
                    this.WorldServer.SendEvent(
                        new EventData((byte)ServerEventCode.CharacterRegister, para),
                        new SendParameters());
                }
            }
        }

        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            // Log.DebugFormat("Trying to disconnect client {0}:{1}", clientPeer.PeerId, clientPeer.ClientData<CharacterData>().CharacterId.Value);
            var para = new Dictionary<byte, object>
                           {
                               {
                                   (byte)ClientParameterCode.PeerId, clientPeer.PeerId.ToByteArray()
                               }
                           };

            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                Log.DebugFormat("Sending disconnect for client {0}:{1}", clientPeer.PeerId, clientPeer.ClientData<CharacterData>().CharacterId.Value);
                
                // One chat server
                if (this.ChatServer != null)
                {
                    this.ChatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterDeRegister, para), new SendParameters());
                }

                // Dynamic Server
                if (this.WorldServer != null)
                {
                    this.WorldServer.SendEvent(new EventData((byte)ServerEventCode.CharacterDeRegister, para), new SendParameters());
                }
            }

            this.LoginServer.SendEvent(new EventData((byte)ServerEventCode.UserLoggedOut, para), new SendParameters());
        }

        public override void ResetServers()
        {
            if (this.ChatServer != null && this.ChatServer.ServerType != (int)ServerType.Chat)
            {
                PhotonServerPeer peer =
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.Chat);
                
                if (peer != null)
                {
                    this.ChatServer = peer;
                }
            }

            if (this.LoginServer != null && this.LoginServer.ServerType != (int)ServerType.Login)
            {
                var peer =
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.Login);

                if (peer != null)
                {
                    this.LoginServer = peer;
                }
            }

            if (this.WorldServer != null && this.WorldServer.ServerType != (int)ServerType.World)
            {
                var peer =
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.World);

                if (peer != null)
                {
                    this.LoginServer = peer;
                }
            }

            // Look for dedicated Chat server
            if (this.ChatServer == null || this.ChatServer.ServerId == null)
            {
             
                this.ChatServer = 
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.Chat) 
                    ?? // if no dedicated chat server look for any that can handle as a chat server
                    Servers.Values.FirstOrDefault(subServerPeer => (subServerPeer.ServerType & (int)ServerType.Chat) != 0);
            }

            if (this.LoginServer == null || this.LoginServer.ServerId == null)
            {
                this.LoginServer =
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.Login)
                    ?? // if no dedicated login server look for any that can handle as a login server
                    Servers.Values.FirstOrDefault(subServerPeer => (subServerPeer.ServerType & (int)ServerType.Login) != 0);
            }

            if (this.WorldServer == null || this.WorldServer.ServerId == null)
            {
                this.WorldServer =
                    Servers.Values.FirstOrDefault(subServerPeer => subServerPeer.ServerType == (int)ServerType.World)
                    ?? // if no dedicated login server look for any that can handle as a login server
                    Servers.Values.FirstOrDefault(subServerPeer => (subServerPeer.ServerType & (int)ServerType.World) != 0);
            }

            // Log out which server connected
            if (this.LoginServer != null)
            {
                this.Log.DebugFormat("Login Server: {0}", this.LoginServer.ConnectionId);
            }

            if (this.ChatServer != null)
            {
                this.Log.DebugFormat("Chat Server: {0}", this.ChatServer.ConnectionId);
            }

            if (this.WorldServer != null)
            {
                this.Log.DebugFormat("World Server: {0}", this.WorldServer.ConnectionId);
            }
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            Log.DebugFormat("Recieved init request to {0}:{1}-{2}", initRequest.LocalIP, initRequest.LocalPort, initRequest);
           
            return initRequest.LocalPort == 4520;
        }

        // Look for server to pass messages to
        public override PhotonServerPeer OnGetServerByType(int serverType, params object[] additional)
        {
            PhotonServerPeer server = null;

            // convert to servertype enum
            switch ((ServerType)Enum.ToObject(typeof(ServerType), serverType))
            {
                case ServerType.Login:
                    if (this.LoginServer != null)
                    {
                        Log.DebugFormat("Found Login Server");
                        server = this.LoginServer;
                    }

                    break;
                case ServerType.Chat:
                    if (this.ChatServer != null)
                    {
                        Log.DebugFormat("Found Chat Server");
                        server = this.ChatServer;
                    }

                    break;
                case ServerType.World:
                    if (this.WorldServer != null)
                    {
                        Log.DebugFormat("Found World Server");
                        server = this.WorldServer;
                    }

                    break;
            }
            return server;
        }
        
        public override void DisconnectAll()
        {
            foreach (var photonServerPeer in this.Servers)
            {
                photonServerPeer.Value.Disconnect();
            }

            foreach (var photonClientPeer in this.Clients)
            {
                photonClientPeer.Value.Disconnect();
            }
        }
    }
}
