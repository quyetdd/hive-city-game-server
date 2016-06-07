// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PhotonResponse.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the PhotonResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Photon.Application
{
    using System.Collections.Generic;

    using HiveCity.Server.Framework;

    public class PhotonResponse : IMessage
    {
        private readonly byte code;
        private readonly Dictionary<byte, object> parameters;
        private readonly int? subCode;
        private readonly string debugMessage;
        private readonly short returnCode;

        public PhotonResponse(
            byte code,
            int? subCode,
            Dictionary<byte, object> parameters,
            string debugMessage,
            short returnCode)
        {
            this.code = code;
            this.parameters = parameters;
            this.subCode = subCode;
            this.debugMessage = debugMessage;
            this.returnCode = returnCode;
        }

        public MessageType Type
        {
            get { return MessageType.Response; }
        }

        public byte Code
        {
            get { return this.code; }
        }

        public string DebugMessage
        {
            get { return this.debugMessage; }
        }

        public short ReturnCode
        {
            get { return this.returnCode; }
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
