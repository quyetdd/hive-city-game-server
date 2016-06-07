// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientParameterCode.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClientParameterCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common
{
    // Don't go higher than 256 codes
    public enum ClientParameterCode : byte
    {
        SubOperationCode = 0,
        PeerId,
        CharacterId,
        UserName,
        Password,
        Email,
        UserId,
        CharacterSlots,
        CharacterList,
        CharacterCreateDetails,
        Object,
        ObjectId
    }
}
