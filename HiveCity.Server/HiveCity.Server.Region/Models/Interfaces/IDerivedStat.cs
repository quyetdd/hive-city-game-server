// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDerivedStat.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDerivedStat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using System.Collections.Generic;

    public interface IDerivedStat : IStat
    {
        IEnumerable<IFunction> Functions { get; }
    }
}
