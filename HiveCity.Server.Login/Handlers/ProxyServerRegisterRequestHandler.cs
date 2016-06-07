// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProxyServerRegisterRequestHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProxyServerRegisterRequestHandler type.
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
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class ProxyServerRegisterRequestHandler : PhotonServerHandler
    {
        public ProxyServerRegisterRequestHandler(PhotonApplication application)
            : base(application)
        {
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
            get { return (int)MessageSubCode.Register; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var peerId = new Guid((byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);

            var operation = new RegisterSecurely(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(
                    new OperationResponse(
                        message.Code,
                        new Dictionary<byte, object>
                            {
                        {
                            (byte) ClientParameterCode.PeerId, peerId.ToByteArray()
                        }
                    })
                {
                    ReturnCode = (int)ErrorCode.OperationInvalid,
                    DebugMessage = operation.GetErrorMessage()
                },
                new SendParameters());

                return true;
            }

            if (string.IsNullOrEmpty(operation.UserName) || string.IsNullOrEmpty(operation.Password) ||
                string.IsNullOrEmpty(operation.Email))
            {
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
                    ReturnCode = (int)ErrorCode.OperationInvalid,
                    DebugMessage = "All fields are required!"
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
                            Log.DebugFormat("Found account name already in use.");
                            transaction.Commit();
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int)ErrorCode.UserNameInUse,
                                        DebugMessage = "Account name already in use, please use another"
                                    },
                                new SendParameters());

                            return true;
                        }

                        // 36 Guid - remove for 32 - hex 
                        var salt = Guid.NewGuid().ToString().Replace("-", string.Empty);
                        Log.DebugFormat("Created salt {0}", salt);

                        var newUser = new User
                        {
                            Email = operation.Email,
                            Username = operation.UserName,
                            Password = BitConverter.ToString(SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(salt + operation.Password))).Replace("-", string.Empty),
                            Salt = salt,
                            Algorithm = "sha1",
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };

                        Log.DebugFormat("Built user object");
                        session.Save(newUser);
                        Log.DebugFormat("Saved new user");
                        transaction.Commit();
                    }

                    using (var transaction = session.BeginTransaction())
                    {
                        Log.DebugFormat("Looking up newly created user.");

                        var userList = session.QueryOver<User>().Where(w => w.Username == operation.UserName).List();

                        if (userList.Any())
                        {
                            // Contains no of characters allowed
                            Log.DebugFormat("Creating Profile");

                            var profile = new UserProfile()
                            {
                                CharacterSlots = 8,
                                UserId = userList[0]
                            };

                            session.Save(profile);
                            Log.DebugFormat("Saved profile");
                            transaction.Commit();
                        }
                    }

                    serverPeer.SendOperationResponse(
                        new OperationResponse(message.Code)
                            {
                                ReturnCode = (byte)ClientReturnCode.UserCreated
                            },
                        new SendParameters());
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Occured", ex);
                serverPeer.SendOperationResponse(
                    new OperationResponse(
                        message.Code,
                        new Dictionary<byte, object>
                            {
                        {
                            (byte) ClientParameterCode.PeerId, peerId.ToByteArray()
                        }
                    })
                {
                    ReturnCode = (int)ErrorCode.UserNameInUse,

                    // Todo: Remove later so client unable to hack 
                    DebugMessage = ex.ToString()
                },
                new SendParameters());
            }

            return true; // processed

        }
    }
}
