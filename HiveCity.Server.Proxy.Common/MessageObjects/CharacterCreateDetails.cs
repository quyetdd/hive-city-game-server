// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CharacterCreateDetails.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the CharacterCreateDetails type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common.MessageObjects
{
    public class CharacterCreateDetails
    {
        public string CharacterName { get; set; }

        public string Chapter { get; set; }

        public string CharacterClass { get; set; }
    }
}
