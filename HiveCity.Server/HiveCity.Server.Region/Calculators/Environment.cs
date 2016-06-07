// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Environment.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the Environment type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Calculators
{
    using HiveCity.Server.Region.Models.Interfaces;

    public class Environment
    {
        public ICharacter Character { get; set; }

        public ICharacter Target { get; set; } // Target check of stats

        public float Value { get; set; }
    }
}
