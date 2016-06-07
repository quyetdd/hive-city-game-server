// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITeleportType.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITeleportType type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    public interface ITeleportType
    {
        Position GetNearestTeleportLocation(ICharacter character);
    }
}
