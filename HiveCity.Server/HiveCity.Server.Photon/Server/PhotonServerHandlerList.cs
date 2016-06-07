// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonServerHandlerList.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonServerHandlerList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Server
{
    using System.Collections.Generic;

    using ExitGames.Logging;

    using HiveCity.Server.Framework;
    using HiveCity.Server.Photon.Application;

    public class PhotonServerHandlerList
    {
        protected readonly ILogger Log;

        private readonly DefaultRequestHandler defaultRequestHandler;
        private readonly DefaultResponseHandler defaultResponseHandler;
        private readonly DefaultEventHandler defaultEventHandler;

        private readonly Dictionary<int, PhotonServerHandler> requestHandlerList;
        private readonly Dictionary<int, PhotonServerHandler> responseHandlerList;
        private readonly Dictionary<int, PhotonServerHandler> eventHandlerList;

        public PhotonServerHandlerList(
            IEnumerable<PhotonServerHandler> handlers,
            DefaultRequestHandler defaultRequestHandler,
            DefaultResponseHandler defaultResponseHandler,
            DefaultEventHandler defaultEventHandler, 
            PhotonApplication application)
        {
            Log = application.Log;

            this.defaultRequestHandler = defaultRequestHandler;
            this.defaultResponseHandler = defaultResponseHandler;
            this.defaultEventHandler = defaultEventHandler;

            this.requestHandlerList = new Dictionary<int, PhotonServerHandler>();
            this.responseHandlerList = new Dictionary<int, PhotonServerHandler>();
            this.eventHandlerList = new Dictionary<int, PhotonServerHandler>();

            foreach (var handler in handlers)
            {
                if (!this.RegisterHandler(handler))
                {
                    this.Log.WarnFormat("Attempted to register handler {0} for type {1}:{2}", handler.GetType().Name, handler.Type, handler.Code);
                }
            }
        }

        public bool RegisterHandler(PhotonServerHandler handler)
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

            if ((handler.Type & MessageType.Response) == MessageType.Response)
            {
                if (handler.SubCode.HasValue && !this.responseHandlerList.ContainsKey(handler.SubCode.Value))
                {
                    this.responseHandlerList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!this.responseHandlerList.ContainsKey(handler.Code))
                {
                    this.responseHandlerList.Add(handler.Code, handler);
                }
                else
                {
                    this.Log.ErrorFormat("ResponseHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }

            if ((handler.Type & MessageType.Async) == MessageType.Async)
            {
                if (handler.SubCode.HasValue && !this.responseHandlerList.ContainsKey(handler.SubCode.Value))
                {
                    this.eventHandlerList.Add(handler.SubCode.Value, handler);
                    registered = true;
                }
                else if (!this.eventHandlerList.ContainsKey(handler.Code))
                {
                    this.eventHandlerList.Add(handler.Code, handler);
                }
                else
                {
                    this.Log.ErrorFormat("EventHandler list already contains handler for {0} - cannot add {1}", handler.Code, handler.GetType().Name);
                }
            }

            return registered;
        }

        public bool HandleMessage(IMessage message, PhotonServerPeer peer)
        {
            bool handled = false;

            switch (message.Type)
            {
                case MessageType.Request:
                    if (message.SubCode.HasValue && this.requestHandlerList.ContainsKey(message.SubCode.Value))
                    {
                        this.requestHandlerList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && this.requestHandlerList.ContainsKey(message.Code))
                    {
                        this.requestHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        this.defaultRequestHandler.HandleMessage(message, peer);
                    }
                    break;

                case MessageType.Response:
                    if (message.SubCode.HasValue && this.responseHandlerList.ContainsKey(message.SubCode.Value))
                    {
                        this.responseHandlerList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && this.responseHandlerList.ContainsKey(message.Code))
                    {
                        this.responseHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        this.defaultResponseHandler.HandleMessage(message, peer);
                    }
                    break;

                case MessageType.Async:
                    if (message.SubCode.HasValue && this.eventHandlerList.ContainsKey(message.SubCode.Value))
                    {
                        this.eventHandlerList[message.SubCode.Value].HandleMessage(message, peer);
                        handled = true;
                    }
                    else if (!message.SubCode.HasValue && this.eventHandlerList.ContainsKey(message.Code))
                    {
                        this.eventHandlerList[message.Code].HandleMessage(message, peer);
                        handled = true;
                    }
                    else
                    {
                        this.defaultEventHandler.HandleMessage(message, peer);
                    }
                    break;
            }

            return handled;
        }
    }
}