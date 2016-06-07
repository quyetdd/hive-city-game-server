// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterSelectItem.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharacterListItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    using System;

    [Serializable]
    public class CharacterListItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public string Class { get; set; }

        public string Chapter { get; set; }
    }
}
