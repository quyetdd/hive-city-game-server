// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageSubCode.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageSubCode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Proxy.Common
{
    public enum MessageSubCode
    {
        // Login Server Code
        Register,
        Login,
        ListCharacters,
        SelectCharacter,
        CreateCharacter,

        // Chat Server Code
        Chat,

        // World Server Code
        RegionInfo,

        // Region Server Code
        CharacterInfo,
        DeleteObject,
        MoveToLocation,
        PlayerInGame,
        PlayerMovement,
        StatusUpdate,
        StopMove,
        TeleportToLocation,
        UserInfo,
  
    }
}
