// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserInfo.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the UserInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class UserInfo
    {
        public PositionData Position { get; set; }

        public string Name { get; set; }
    }
}
