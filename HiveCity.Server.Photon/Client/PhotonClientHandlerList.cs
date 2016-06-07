// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonClientHandlerList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonClientHandlerList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Client
{
    using System.Collections.Generic;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;

    public class PhotonClientHandlerList
    {
        protected readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<int, PhotonClientHandler> requestHandlerList;

        public PhotonClientHandlerList(IEnumerable<PhotonClientHandler> handlers)
        {
            this.requestHandlerList = new Dictionary<int, PhotonClientHandler>();

            foreach (var handler in handlers)
            {
                if (!this.RegisterHandler(handler))
                {
                    this.Log.WarnFormat("Attempted to register handler {0} for type {1}:{2}", handler.GetType().Name, handler.Type, handler.Code);
                }
            }
        }

        public bool RegisterHandler(PhotonClientHandler handler)
        {
            var registered = false;

            if ((handler.Type & MessageType.Request) == MessageType.Request)
            {
                if (handler.SubCode.HasValue && !this.requestHandlerList.ContainsKey(handler.SubCode.Value))
                {
                    this.requestHandlerList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!this.requestHandlerList.ContainsKey(handler.Code))
                {
                    this.requestHandlerList.Add(handler.Code, handler);
                }
                else
                {
                    this.Log.ErrorFormat("RequestHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }

            return registered;
        }

        public bool HandleMessage(IMessage message, PhotonClientPeer peer)
        {
            var handled = false;

            switch (message.Type)
            {
                case MessageType.Request:
                    //if (message.SubCode.HasValue && _requestHandlerList.ContainsKey(message.SubCode.Value))
                    //{
                    //    _requestHandlerList[message.SubCode.Value].HandleMessage(message, peer);
                    //    handled = true;
                    //}
                    //else 
                    if (this.requestHandlerList.ContainsKey(message.Code))
                    {
                        this.requestHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        Log.DebugFormat("PhotonClientHandlerList - Message Handler not found!");
                    }

                    break;
            }

            return handled;
        }
    }
}
