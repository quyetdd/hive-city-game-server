// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICondition.cs" company="HiveCity">
//   Copyright © 2014 HiveCity. All rights reserved.
// </copyright>
// <summary>
//   Defines the ICondition type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HiveCity.Server.Region.Models.Interfaces
{
    using HiveCity.Server.Region.Calculators;

    public interface ICondition
    {
        bool Test(Environment env);

        void NotifyChanged();
    }
}
