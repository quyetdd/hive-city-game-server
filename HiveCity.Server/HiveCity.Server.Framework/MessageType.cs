// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageType.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Framework
{
    using System;

    [Flags] // it's a request and event
    public enum MessageType
    {
        Request = 0x1,
        Response = 0x2,
        Async = 0x4
    }
}
