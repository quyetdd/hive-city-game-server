﻿using System.IO;
using System.Xml.Serialization;
using HiveCity.Server.Framework;
using HiveCity.Server.Photon.Application;
using HiveCity.Server.Photon.Server;
using HiveCity.Server.Proxy.Common;
using HiveCity.Server.Sub.Common.Data;
using HiveCity.Server.Sub.Common.Operations;
using Photon.SocketServer;

namespace HiveCity.Server.Sub.Common.Handlers
{
    public class HandleServerRegistration : PhotonServerHandler
    {
        public HandleServerRegistration(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ServerOperationCode.RegisterSubServer; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            OperationResponse operationResponse;
            if (serverPeer.ServerId.HasValue)
            {
                operationResponse = new OperationResponse(message.Code)
                {
                    ReturnCode = -1,
                    DebugMessage = "Already Registered"
                };
            }
            else
            {
                var registerRequest = new RegisterSubServer(serverPeer.Protocol, message);
                if (!registerRequest.IsValid)
                {
                    string msg = registerRequest.GetErrorMessage();
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Invalid Register Request: {0}", msg);
                    }
                    operationResponse = new OperationResponse(message.Code)
                    {
                        DebugMessage = msg,
                        ReturnCode = (short)ErrorCode.OperationInvalid
                    };
                }
                else
                {
                    XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
                    StringReader inStream = new StringReader(registerRequest.RegisterSubServerOperation);
                    var registerData = (RegisterSubServerData)mySerializer.Deserialize(inStream);

                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Received register request: Address={0}, UdpPort={2}, TcpPort={1}, Server Type={3}", registerData.GameServerAddress, registerData.UdpPort, registerData.TcpPort, registerData.ServerType);
                    }

                    if (registerData.UdpPort.HasValue)
                    {
                        serverPeer.UdpAddress = registerData.GameServerAddress + ":" + registerData.UdpPort;
                    }
                    if (registerData.TcpPort.HasValue)
                    {
                        serverPeer.TcpAddress = registerData.GameServerAddress + ":" + registerData.TcpPort;
                    }

                    serverPeer.ServerId = registerData.ServerId;
                    serverPeer.ServerType = registerData.ServerType;
                    serverPeer.ApplicationName = registerData.ApplicationName;

                    Server.ConnectionCollection<PhotonConnectionCollection>().OnConnect(serverPeer);

                    operationResponse = new OperationResponse(message.Code);
                }
            }

            serverPeer.SendOperationResponse(operationResponse, new SendParameters());
            return true;
        }
    }
}
