// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandleClientRegionRequests.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the HandleClientRegionRequests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Handlers
{
    using System;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;

    using global::Photon.SocketServer;

    public class HandleClientRegionRequests : PhotonClientHandler
    {
        public HandleClientRegionRequests(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Region; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonClientPeer clientPeer)
        {
            // Anti hack remove peerid if has been set
            message.Parameters.Remove((byte)ClientParameterCode.PeerId);
            message.Parameters.Remove((byte)ClientParameterCode.UserId);
            message.Parameters.Add((byte)ClientParameterCode.PeerId, clientPeer.PeerId.ToByteArray());

            if (clientPeer.ClientData<CharacterData>().UserId.HasValue)
            {
                message.Parameters.Add((byte)ClientParameterCode.UserId, clientPeer.ClientData<CharacterData>().UserId);
            }
            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                // character selection to pass the character id
                message.Parameters.Remove((byte)ClientParameterCode.CharacterId);
                message.Parameters.Add((byte)ClientParameterCode.CharacterId, clientPeer.ClientData<CharacterData>().CharacterId);
            }

            var operationRequest = new OperationRequest(message.Code, message.Parameters);
            switch (message.Code)
            {
                case (byte)ClientOperationCode.Region:
                    if (Server.ConnectionCollection<PhotonConnectionCollection>() != null)
                    {
                        try
                        {
                            // Send to the world server to deal with character region
                         Server.ConnectionCollection<PhotonConnectionCollection>().OnGetServerByType((int)ServerType.World).SendOperationRequest(operationRequest, new SendParameters());
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("This ain't working {0}", ex.ToString());
                        }
                    }
                    break;
                default:
                    Log.DebugFormat("Invalid Operation Code - Expecting Region");
                    break;
            }

            return true;
        }
    }
}