// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginServerSelectCharacterHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginServerSelectCharacterHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Login.Operations;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class LoginServerSelectCharacterHandler : PhotonServerHandler
    {
        public LoginServerSelectCharacterHandler(PhotonApplication application)
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
            get { return (int)MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var para = new Dictionary<byte, object>
            {
                // return message to the peer that sent the request
                {
                    (byte)ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]
                },
                {
                    (byte)ClientParameterCode.SubOperationCode, message.Parameters[(byte)ClientParameterCode.SubOperationCode]
                }
            };

            var operation = new SelectCharacter(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                Log.ErrorFormat(operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                        {
                            ReturnCode = (int)ErrorCode.OperationInvalid,
                            DebugMessage = operation.GetErrorMessage(),
                            Parameters = para
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
                        var user = session.QueryOver<User>().Where(w => w.Id == operation.UserId).List().FirstOrDefault();

                        if (user != null)
                        {
                            Log.DebugFormat("Found user {0}", user.Username);
                        }

                        var character = session.QueryOver<PlayerCharacter>()
                                               .Where(w => w.UserId == user)
                                               .And(a => a.Id == operation.CharacterId)
                                               .List()
                                               .FirstOrDefault();
                        transaction.Commit();

                        if (character == null)
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int)ErrorCode.InvalidCharacter,
                                        DebugMessage = "Invalid Character",
                                        Parameters = para
                                    },
                                new SendParameters());
                        }
                        else
                        {
                            Log.DebugFormat("Found Character {0}", character.Name);
                            para.Add((byte)ClientParameterCode.CharacterId, character.Id); // added for chat server etc
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (byte)ErrorCode.Ok,
                                        Parameters = para
                                    },
                                new SendParameters());
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Log.Error(e);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                        {
                            ReturnCode = (int)ErrorCode.InvalidCharacter,
                            DebugMessage = e.ToString(),
                            Parameters = para
                        },
                    new SendParameters());
            }

            return true;
        }
    }
}
