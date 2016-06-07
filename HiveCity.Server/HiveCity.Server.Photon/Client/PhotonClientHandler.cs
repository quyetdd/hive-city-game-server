// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonClientHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonClientHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Client
{
    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;

    public abstract class PhotonClientHandler : IHandler<PhotonClientPeer>
    {
        public abstract MessageType Type { get; }

        public abstract byte Code { get; }

        public abstract int? SubCode { get; }

        protected PhotonApplication Server;

        protected ILogger Log = LogManager.GetCurrentClassLogger();

        public PhotonClientHandler(PhotonApplication application)
        {
            this.Server = application;
        }

        public bool HandleMessage(IMessage message, PhotonClientPeer clientPeer)
        {
            this.OnHandleMessage(message, clientPeer);
            return true;
        }

        protected abstract bool OnHandleMessage(IMessage message, PhotonClientPeer clientPeer);

    }
}
