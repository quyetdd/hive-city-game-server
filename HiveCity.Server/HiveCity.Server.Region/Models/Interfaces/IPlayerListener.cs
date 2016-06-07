// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPlayerListener.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPlayerListener type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using System;

    public interface IPlayerListener
    {
        event Action<IPlayer> OnAddPlayer;

        event Action<IPlayer> OnRemovePlayer;
    }
}
