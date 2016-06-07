// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharInfo.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class CharInfo
    {
        public PositionData Position { get; set; }
        public string Name { get; set; }
    }
}
