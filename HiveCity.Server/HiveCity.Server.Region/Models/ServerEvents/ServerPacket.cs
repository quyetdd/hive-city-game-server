// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerPacket.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerPacket type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.ServerEvents
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    using HiveCity.Server.Photon.Application;
    using HiveCity.Server.Proxy.Common;

    public class ServerPacket : PhotonEvent
    {
        public readonly bool SendToSelf;

        public ServerPacket(ClientEventCode code, MessageSubCode subCode, bool sendToSelf = true) 
            : base((byte)code, (int?)subCode, new Dictionary<byte,object>())
        {
            this.SendToSelf = sendToSelf;
            this.AddParameter(subCode, ClientParameterCode.SubOperationCode);
        }

        public void AddParameter<T>(T obj, ClientParameterCode code)
        {
            if (Parameters.ContainsKey((byte) code))
            {
                this.Parameters[(byte)code] = obj;
            }
            else
            {
                Parameters.Add((byte)code, obj);
            }
        }

        public void AddSerializedParameter<T>(T obj, ClientParameterCode code, bool binary = true)
        {
            var serializer = new XmlSerializer(typeof(T));
            var outStream = new StringWriter();

            serializer.Serialize(outStream, obj);

            if (Parameters.ContainsKey((byte)code))
            {
                this.Parameters[(byte)code] = outStream.ToString();
            }
            else
            {
                Parameters.Add((byte)code, outStream.ToString());
            }
        }
    }
}
