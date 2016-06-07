// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStatHolder.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStatHolder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface IStatHolder
    {
        ICharacter Character { get; set; } // The char holder the stats

        Dictionary<Type, IStat> Stats { get; }

        float GetStat<T>() where T : class, IStat;

        float GetStat<T>(T stat) where T : class, IStat;

        void SetStat<T>(float value) where T : class, IStat;

        string SerializeStats();

        void DeserializeStats(string stats);
    }
}
