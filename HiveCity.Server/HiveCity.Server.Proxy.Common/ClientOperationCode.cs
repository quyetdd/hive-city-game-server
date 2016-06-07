// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientOperationCode.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Server Type flags
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common
{
    using System;

    /// <summary>
    /// Server Type flags
    /// </summary>
    [Flags]
    public enum ClientOperationCode
    {
        Chat = 0x1,
        Login = 0x2,
        Region = 0x4
    }
}
