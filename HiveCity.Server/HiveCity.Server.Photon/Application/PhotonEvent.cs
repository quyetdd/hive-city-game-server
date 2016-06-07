// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonEvent.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonEvent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System.Collections.Generic;

    using HiveCity.Server.Framework;

    public class PhotonEvent : IMessage
    {
        private readonly byte code;
        private readonly Dictionary<byte, object> parameters;
        private readonly int? subCode;

        public PhotonEvent(byte code, int? subCode, Dictionary<byte, object> parameters)
        {
            this.code = code;
            this.parameters = parameters;
            this.subCode = subCode;
        }

        public MessageType Type
        {
            get { return MessageType.Async; }
        }

        public byte Code
        {
            get { return this.code; }
        }

        public int? SubCode
        {
            get { return this.subCode; }
        }

        public Dictionary<byte, object> Parameters
        {
            get { return this.parameters; }
        }
    }
}
