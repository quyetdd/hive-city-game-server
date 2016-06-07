// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHandler.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    // Handle Messages
    public interface IHandler<in T>
    {
        MessageType Type { get; }

        byte Code { get; } // parse the message

        int? SubCode { get; }

        bool HandleMessage(IMessage message, T peer);
    }
}
