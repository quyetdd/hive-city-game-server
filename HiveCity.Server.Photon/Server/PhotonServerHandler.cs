// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonServerHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonServerHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;

    public abstract class PhotonServerHandler : IHandler<PhotonServerPeer>
    {
        public abstract MessageType Type { get; }

        public abstract byte Code { get; }

        public abstract int? SubCode { get; }

        protected PhotonApplication Server;

        protected ILogger Log;

        protected PhotonServerHandler(PhotonApplication application)
        {
            this.Server = application;
            this.Log = this.Server.Log;
        }
        
        public bool HandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            this.OnHandleMessage(message, serverPeer);
            return true;
        }

        protected abstract bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer);
    }
}