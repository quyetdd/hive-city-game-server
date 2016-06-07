// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChatItem.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChatItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class ChatItem
    {
        public ChatType Type { get; set; }

        public string TellPlayer { get; set; }

        public string Text { get; set; }
    }
}
