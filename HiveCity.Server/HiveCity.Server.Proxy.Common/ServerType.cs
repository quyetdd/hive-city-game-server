// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerType.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServerType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common
{
    using System;

    [Flags]
    public enum ServerType
    {
        Proxy = 0x1,
        Login = 0x2,
        Chat = 0x4,
        Region = 0x8,
        World = 0x10
    }
}
