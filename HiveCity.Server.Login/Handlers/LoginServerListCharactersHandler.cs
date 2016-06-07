// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginServerListCharactersHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginServerListCharactersHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Handlers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Login.Operations;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Proxy.Common.MessageObjects;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class LoginServerListCharactersHandler : PhotonServerHandler
    {
        public LoginServerListCharactersHandler(PhotonApplication application)
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
            get { return (byte)MessageSubCode.ListCharacters; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new ListCharacters(serverPeer.Protocol, message);

            if (!operation.IsValid)
            {
                Log.DebugFormat("Invalid Operation - {0}", operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                        {
                            ReturnCode = (int)ErrorCode.OperationInvalid,
                            DebugMessage = operation.GetErrorMessage()
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
                        var user =
                            session.QueryOver<User>().Where(w => w.Id == operation.UserId).List().FirstOrDefault();

                        if (user != null)
                        {
                            var profile =
                                session.QueryOver<UserProfile>().Where(w => w.UserId == user).List().FirstOrDefault();

                            if (profile != null)
                            {
                                var para = new Dictionary<byte, object>()
                                {
                                    {(byte) ClientParameterCode.CharacterSlots, profile.CharacterSlots},
                                    {
                                        (byte) ClientParameterCode.PeerId,
                                        message.Parameters[(byte) ClientParameterCode.PeerId]
                                    },
                                    {
                                        (byte) ClientParameterCode.SubOperationCode,
                                        message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                    }
                                };

                                var characters = session.QueryOver<PlayerCharacter>().Where(w => w.UserId == user).List();
                                var characterSerializer = new XmlSerializer(typeof(CharacterListItem));

                                var characterList = new Hashtable();
                                foreach (var playerCharacter in characters)
                                {
                                    var outStream = new StringWriter();
                                    characterSerializer.Serialize(outStream, playerCharacter.BuildCharacterListItem());
                                    characterList.Add(playerCharacter.Id, outStream.ToString());
                                }

                                para.Add((byte)ClientParameterCode.CharacterList, characterList);
                                transaction.Commit();

                                serverPeer.SendOperationResponse(
                                    new OperationResponse((byte)ClientOperationCode.Login)
                                        {
                                            Parameters = para
                                        },
                                new SendParameters());
                            }
                            else
                            {
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                        {
                                            ReturnCode = (int)ErrorCode.OperationInvalid,
                                            DebugMessage = "Profile not found"
                                        },
                                   new SendParameters());
                            }
                        }
                        else
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int)ErrorCode.OperationInvalid,
                                        DebugMessage = "User not found"
                                    },
                                new SendParameters());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                        {
                            ReturnCode = (int)ErrorCode.OperationInvalid,
                            DebugMessage = e.ToString()
                        },
                    new SendParameters());
                throw;
            }

            return true;
        }
    }
}
