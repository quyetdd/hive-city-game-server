// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IMessage.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    using System.Collections.Generic;

    public interface IMessage
    {
        MessageType Type { get; }

        byte Code { get; } // Operation Code

        int? SubCode { get; } // async event goes to a subserver for example message to chat server

        Dictionary<byte, object> Parameters { get; }
    }
}
