// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginServerCreateCharacterHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the LoginServerCreateCharacterHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Login.Handlers
{
    using System;
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

    using IMessage = HiveCity.Server.Framework.IMessage;

    public class LoginServerCreateCharacterHandler : PhotonServerHandler
    {
        public LoginServerCreateCharacterHandler(PhotonApplication application)
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
            get { return (int)MessageSubCode.CreateCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var para = new Dictionary<byte, object>
            {
                {
                    (byte)ClientParameterCode.PeerId, message.Parameters[(byte)ClientParameterCode.PeerId]
                },
                {
                    (byte)ClientParameterCode.SubOperationCode, message.Parameters[(byte)ClientParameterCode.SubOperationCode]
                }
            };

            var operation = new CreateCharacter(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
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
                        var profile = session.QueryOver<UserProfile>().Where(w => w.UserId == user).List().FirstOrDefault();
                        var characters = session.QueryOver<PlayerCharacter>().Where(w => w.UserId == user).List();

                        // Limit character creation due to subscription maybe?
                        if (profile != null && profile.CharacterSlots <= characters.Count())
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int)ErrorCode.InvalidCharacter,
                                        DebugMessage = "No free character slots",
                                        Parameters = para
                                    },
                                new SendParameters());
                        }
                        else
                        {
                            var mySerializer = new XmlSerializer(typeof(CharacterCreateDetails));
                            var outStream = new StringReader(operation.CharacterCreateDetails);
                            var createCharacter = mySerializer.Deserialize(outStream) as CharacterCreateDetails;
                            var character = session.QueryOver<PlayerCharacter>()
                                                   .Where(w => w.Name == createCharacter.CharacterName)
                                                   .List()
                                                   .FirstOrDefault();

                            if (character != null)
                            {
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                        {
                                            ReturnCode = (int)ErrorCode.InvalidCharacter,
                                            DebugMessage = "Character name already exists",
                                            Parameters = para
                                        },
                                    new SendParameters());
                            }
                            else
                            {
                                var newCharacter = new PlayerCharacter
                                {
                                    UserId = user,
                                    Class = createCharacter.CharacterClass,
                                    Name = createCharacter.CharacterName,
                                    Chapter = createCharacter.Chapter,
                                    Level = 1
                                };

                                session.Save(newCharacter);
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                        {
                                            ReturnCode = (int)ErrorCode.Ok,
                                            ////DebugMessage = "Character charactered",
                                            Parameters = para
                                        },
                                    new SendParameters());
                            }
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
