// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFunction.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFunction type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using HiveCity.Server.Region.Calculators;

    public interface IFunction
    {
        IStat Stat { get; }

        int Order { get; }

        CObject Owner { get; set; }

        ICondition Condition { get; set; }

        void Calc(Environment env);
    }
}
