// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Level.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the Level type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Stats
{
    using HiveCity.Server.Region.Models.Interfaces;

    public class Level : IStat
    {
        public string Name
        {
            get { return "Level"; }
        }

        public bool IsBaseStat
        {
            get { return true; }
        }

        public bool IsNonZero
        {
            get { return true; }
        }

        public float BaseValue { get; set; }
    }
}
