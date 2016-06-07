// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStat.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    public interface IStat
    {
        string Name { get; }

        bool IsBaseStat { get; } // Dereived stat

        bool IsNonZero { get; } // Never go below zero

        float BaseValue { get; set; }
    }
}
