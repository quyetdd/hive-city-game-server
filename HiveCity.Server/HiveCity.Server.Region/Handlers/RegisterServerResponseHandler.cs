// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterServerResponseHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the RegisterServerResponseHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Handlers
{
    using System.Collections.Generic;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Photon.Server;
    using HiveCity.Server.Proxy.Common;
    using HiveCity.Server.Sub.Common;
    using HiveCity.Server.Sub.Common.Data.NHibernate;

    using global::Photon.SocketServer;

    public class RegisterServerResponseHandler : PhotonServerHandler
    {
        public RegisterServerResponseHandler(PhotonApplication application)
            : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
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
            var para = new Dictionary<byte, object>()
                           {
                               {
                                   (byte)ClientParameterCode.SubOperationCode,
                                   MessageSubCode.RegionInfo
                               }
                           };

            // Ask db for region boundaries
            using (var session = NHibernateHelper.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var region =
                        session.QueryOver<RegionRecord>()
                            .Where(w => w.Name == Server.ApplicationName)
                            .SingleOrDefault();

                    if (region != null)
                    {
                        para.Add((byte)ClientParameterCode.Object, region.Boundary);
                    }
                }
            }

            serverPeer.SendEvent(new EventData((byte)ClientEventCode.ServerPacket, para), new SendParameters());

            return true;
        }
    }
}
