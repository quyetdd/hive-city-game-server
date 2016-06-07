// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyServerLoginRequestHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProxyServerLoginRequestHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Login.Operations;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Client;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.ClientData;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class ProxyServerLoginRequestHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory clientFactory;

        public ProxyServerLoginRequestHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory)
            : base(application)
        {
            this.clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int)MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new LoginSecurely(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(
                    new OperationResponse(
                        message.Code,
                        new Dictionary<byte, object>
                            {
                                {
                                    (byte)ClientParameterCode.PeerId,
                                    message.Parameters[(byte)ClientParameterCode.PeerId]
                                }
                            })
                        {
                            // Todo change for production
                            ReturnCode = (int)ErrorCode.OperationInvalid,
                            DebugMessage = operation.GetErrorMessage()
                        },
                    new SendParameters());

                return true;
            }

            if (string.IsNullOrEmpty(operation.UserName) || string.IsNullOrEmpty(operation.Password))
            {
                serverPeer.SendOperationResponse(
                    new OperationResponse(
                        message.Code,
                        new Dictionary<byte, object>
                            {
                                {
                                    (byte)ClientParameterCode.PeerId,
                                    message.Parameters[(byte)ClientParameterCode.PeerId]
                                }
                            })
                        {
                            ReturnCode = (int)ErrorCode.InCorrectUserNameOrPassword,
                            DebugMessage = "Username or password is incorrect!"
                        },
                    new SendParameters());

                return true;
            }

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        Log.Debug("About to look for user account");
                        var userList = session.QueryOver<User>().Where(w => w.Username == operation.UserName).List();
                        if (userList.Any())
                        {
                            var user = userList[0];
                            var hash = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(user.Salt + operation.Password))).Replace("-", string.Empty);

                            if (string.Equals(hash.Trim(), user.Password.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                var server = Server as LoginServer;

                                if (server != null)
                                {
                                    bool foundUser = false;
                                    foreach (var subServerClientPeer in server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                                    {
                                        if (subServerClientPeer.Value.ClientData<CharacterData>().UserId == user.Id)
                                        {
                                            foundUser = true;
                                        }
                                    }

                                    var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                                    if (foundUser)
                                    {
                                        var para = new Dictionary<byte, object>()
                                        {
                                            {
                                                (byte)ClientParameterCode.PeerId, peerId.ToByteArray()
                                            },
                                            {
                                                (byte)ClientParameterCode.SubOperationCode, message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                            }
                                        };

                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte)ClientOperationCode.Login)
                                            {
                                                Parameters = para,
                                                ReturnCode = (short)ErrorCode.UserCurrentlyLoggedIn,
                                                DebugMessage = "User is currently logged in!"
                                            },
                                            new SendParameters());
                                    }
                                    else
                                    {
                                        Log.Debug("Login Handler successfully found user to login.");
                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients.Add(peerId, this.clientFactory(peerId));
                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients[peerId].ClientData<CharacterData>().UserId = user.Id;

                                        var para = new Dictionary<byte, object>()
                                        {
                                            {
                                                (byte)ClientParameterCode.PeerId, peerId.ToByteArray()
                                            },
                                            {
                                                (byte)ClientParameterCode.SubOperationCode,message.Parameters[(byte)ClientParameterCode.SubOperationCode]
                                            },
                                            {
                                                (byte)ClientParameterCode.UserId, user.Id
                                            }
                                        };

                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte)ClientOperationCode.Login)
                                            {
                                                Parameters = para
                                            },
                                            new SendParameters());
                                    }
                                }

                                return true;
                            }
                            else
                            {
                                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                                serverPeer.SendOperationResponse(
                                    new OperationResponse(
                                        message.Code,
                                        new Dictionary<byte, object>
                                            {
                                        {
                                            (byte)ClientParameterCode.PeerId, peerId.ToByteArray()
                                        }
                                    })
                                {
                                    ReturnCode = (int)ErrorCode.InCorrectUserNameOrPassword,
                                    DebugMessage = "Username or password is incorrect!"
                                },
                                new SendParameters());

                                return true;
                            }
                        }
                        else
                        {
                            Log.DebugFormat("Username does not exist {0}", operation.UserName);
                            transaction.Commit();
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int)ErrorCode.InCorrectUserNameOrPassword,
                                        DebugMessage = "Username or password is incorrect!"
                                    },
                                    new SendParameters());
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Occured", ex);
                var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

                serverPeer.SendOperationResponse(
                    new OperationResponse(
                        message.Code,
                        new Dictionary<byte, object>
                            {
                        {
                            (byte)ClientParameterCode.PeerId, peerId.ToByteArray()
                        }
                    })
                {
                    ReturnCode = (int)ErrorCode.UserNameInUse,

                    // Todo: Remove Exception from debug message response later so client unable to hack 
                    DebugMessage = ex.ToString()
                }, 
                new SendParameters());
            }

            return true; // processed
        }
    }
}
